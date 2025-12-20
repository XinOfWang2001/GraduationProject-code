import logging

import polars as pl
from polars import DataFrame

from const import PREDICTION_FORMAT, TIMESTAMP
from Domain import ModelRequest, ModelResult, PredictionResult
from services.AbstractServices import (ICalculationBuilder, ILagHandler,
                                       InferencePipelineTemplate)

from ..PredictorStrategy.PredictorStrategyPicker import PredictorStrategyPicker
from ..Utils.UtilityFunctions import get_interval


class ForecastInferencePipeline(InferencePipelineTemplate):
    """
    Responsibility of the forecasting InferencePipeline
    - Load data optional
    - Load model
    - Generate forecast.
    """

    lag_handler: ILagHandler
    lag_features: list[str]
    predictor_picker: PredictorStrategyPicker
    calculation_builder: ICalculationBuilder

    def __init__(
        self,
        calculation_builder,
        model_pipeline=None,
        data_pipeline=None,
        lag_handler=None,
        predictor_picker: PredictorStrategyPicker = None,
    ):
        self.lag_handler = lag_handler
        self.predictor_picker = predictor_picker
        self.calculation_builder = calculation_builder
        super().__init__(model_pipeline, data_pipeline)

    async def execute(
        self, model_request: ModelRequest, **optional: dict
    ) -> ModelResult:
        """
        This method will ensure that forecasting data is generated
        """

        return await super().execute(model_request, **optional)

    async def _load_data(self, data_request):
        # At the moment, no feature variables are used for making forecasts, like lagged features
        # Thus no need to load up the current data.
        return await self.data_pipeline.execute(data_request)

    async def _load_model(self, identifier=None):
        # Currently doing nothing. The inserted Pipeline
        return await super()._load_model(identifier)

    def _transform(self, current_data, model_request):
        calculation_runner = self.calculation_builder.add_calculations(model_request).build()
        timestamp_data = self._prepare_datefeatures(model_request)
        self.current_data = calculation_runner.execute(current_data)
        # Copy the current schema of the data retrieved.
        prepared_df = pl.DataFrame(schema=self.current_data.schema)
        # Copy schema
        prepared_df = pl.concat([prepared_df, timestamp_data], how="diagonal")
        return prepared_df

    def _make_predictions(self, model_request):
        logging.info("Made predictions")
        feature_columns = [TIMESTAMP] + [
            col.ColumnName for col in model_request.ModelConfig.Features
        ]
        forecast_input = self.input_data.select(feature_columns)
        past_data = self.current_data
        predicted_result = None
        # Should be refactored
        predictor = self.predictor_picker.choose_predictor(
            model_request, self.model_pipeline, past_data
        )
        predicted_result = predictor.execute(forecast_input)
        # Create DataFrame
        predict_columns = [
            PREDICTION_FORMAT.format(col=col.ColumnName)
            for col in model_request.ModelConfig.Targets
        ]

        predict_df = self._format_predictions(
            predicted_result, self.input_data[TIMESTAMP], predict_columns
        )
        return predict_df
    
    def _format_results(self, model_request):
        return PredictionResult(self.current_data, self.predicted_data, model_request.ModelConfig)

    def _prepare_datefeatures(self, request: ModelRequest) -> DataFrame:
        """
        Based on upper bound of the datacollection and the forecasting date in modelconfiguration.
        Forecasting data will be generated between the stated datetimes.
        """
        df = DataFrame()
        start_date = request.get_lower_bound_date()
        end_date = request.get_upper_predict_date()
        interval_value = get_interval(request.DataRequest.TimelevelRange)

        date_input = pl.datetime_range(
            start_date,
            end_date,
            eager=True,
            interval=interval_value,
            time_zone="UTC",
        )
        return df.with_columns([(date_input).alias(TIMESTAMP)])

    def _format_predictions(
        self, predicted_result, index_col: DataFrame, target_cols: list[str]
    ):
        predict_columns = target_cols
        predict_df = DataFrame(
            data=predicted_result, schema=predict_columns, orient="row"
        )
        predict_df = predict_df.with_columns(
            [pl.Series(name=TIMESTAMP, values=index_col)]
        )

        return predict_df.select([TIMESTAMP] + predict_columns)

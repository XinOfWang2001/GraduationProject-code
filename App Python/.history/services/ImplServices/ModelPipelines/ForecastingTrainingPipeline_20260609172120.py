import logging

import polars as pl
from polars import DataFrame
from sklearn.model_selection import train_test_split

from const import MAPE_FORMAT, PREDICTION_FORMAT, RMSE_FORMAT, TIMESTAMP
from Domain import ModelConfig, ModelRequest, ModelTrainingsResult
from services.AbstractServices import (DataPipelineTemplate,
                                       ICalculationBuilder, ILagHandler,
                                       ModelPipelineTemplate)

from ..ModelBuilder.PipelineBuilder import PipelineBuilder
from ..Utils.MetricsService import MetricsService


class ForecastingTrainingPipeline(ModelPipelineTemplate):

    data_pipeline: DataPipelineTemplate
    pipeline_builder: PipelineBuilder
    lag_handler: ILagHandler
    calculation_builder: ICalculationBuilder
    _input_features: list[str]

    def __init__(
        self,
        data_pipeline: DataPipelineTemplate,
        pipe_builder: PipelineBuilder,
        calculation_builder: ICalculationBuilder,
        lag_handler: ILagHandler = None,
    ):
        self.data_pipeline = data_pipeline
        self.pipeline_builder = pipe_builder
        self.calculation_builder = calculation_builder
        self.lag_handler = lag_handler
        self._input_features = []
        # Temp service
        self.metrics_service = MetricsService()

    async def execute(self, modeling_request):
        return await super().execute(modeling_request)

    async def _extract(self, data_request):
        return await self.data_pipeline.execute(data_request)

    def _transform(self, df: DataFrame, model_request: ModelRequest):
        self.calculation_builder.reset()
        calculation_runner = self.calculation_builder.add_calculations(model_request).build()
        feature_columns = [col.ColumnName for col in model_request.ModelConfig.Features]
        # Perform data transformations
        df = calculation_runner.execute(df)
        df = self.lag_handler.execute_change(df, input_columns=feature_columns)
        self._input_features = [col for col in df.columns if col.__contains__("_lag")]
        logging.debug("selected_lag features as feature columns")
        # Clean up data. Remove all empty records.
        df = df.fill_null(strategy="forward")
        df = df.drop_nulls()
        return df

    def _load_model(self, model_config: ModelConfig):
        # Add further steps to the pipeline
        self.pipeline_builder.add_datetime_features(model_config)
        self.pipeline_builder.add_model(model_config)

    def _train_model(self, df: DataFrame, model_config: ModelConfig = None):
        # Create the pipeline object, in order to use it.
        model_pipeline = self.pipeline_builder.build_pipeline()
        target_strings = model_config.get_targets()
        TARGET = df.select(target_strings)
        # Select target and features for model training
        FEATURES = df.select([TIMESTAMP] + self._input_features)
        # Split training, based to on DataSplitRatio value
        X_TRAIN, X_TEST, Y_TRAIN, Y_TEST = train_test_split(
            FEATURES,
            TARGET,
            test_size=model_config.get_test_percentage(),
            shuffle=False,
        )
        # Train model on trainingset.
        model_pipeline.fit(X_TRAIN, Y_TRAIN)
        # Create model result: With both feature and testsets.
        modeltrainingsresult = ModelTrainingsResult(df, model_pipeline, model_config)
        modeltrainingsresult.set_trainings_set(X_TRAIN, Y_TRAIN)
        modeltrainingsresult.set_test_set(X_TEST, Y_TEST)
        return modeltrainingsresult

    def _validate_model(self, model_result: ModelTrainingsResult):
        test_set = model_result.test_set
        # Perform using the testset
        predictions = model_result.model_pipeline.predict(test_set.features)
        # Append results back to original result
        predicted_columns = [
            PREDICTION_FORMAT.format(col=col) for col in test_set.targets.columns
        ]
        predicted_df = self._append_results_to_index(
            test_set.features, predictions, predicted_columns
        )
        predicted_df = predicted_df.select([TIMESTAMP] + predicted_columns)

        logging.debug(predicted_df)
        model_result.test_set.add_predictions(predicted_df)
        # Apply relevant metrics, based on the model results of the testset
        self._add_forecasting_metrics(model_result)
        # # To fully fit the model
        combined_features = model_result.get_full_feature_data()
        combined_targets = model_result.get_full_target_data()
        model_result.model_pipeline.fit(combined_features, combined_targets)
        return model_result

    def _add_forecasting_metrics(self, model_result: ModelTrainingsResult):
        test_set = model_result.test_set
        # Generate metrics for model validation.
        for col in test_set.targets.columns:
            rmse = self.metrics_service.generate_rmse(
                test_set.targets.select(col),
                test_set.predictions.select(PREDICTION_FORMAT.format(col=col)),
            )
            mape = self.metrics_service.generate_mape(
                test_set.targets.select(col),
                test_set.predictions.select(PREDICTION_FORMAT.format(col=col)),
            )
            # Add metrics to modeltrainingsresult
            model_result.add_metric(RMSE_FORMAT, col, rmse)
            model_result.add_metric(MAPE_FORMAT, col, mape)

    # This is private method only in Forecasting pipeline.
    def _append_results_to_index(
        self, original_data: DataFrame, predicted_values, columns: list
    ):
        predicted_df = DataFrame(data=predicted_values, schema=columns)
        return predicted_df.with_columns(
            [pl.Series(name=TIMESTAMP, values=original_data[TIMESTAMP])]
        )

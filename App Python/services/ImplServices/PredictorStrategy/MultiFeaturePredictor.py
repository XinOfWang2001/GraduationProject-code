import logging

import polars as pl
from polars import DataFrame
from sklearn.pipeline import Pipeline

from const import TIMESTAMP
from Domain import ModelRequestDTO
from services.AbstractServices import ForecastPredictorStrategy, ILagHandler


class MultiFeaturePredictor(ForecastPredictorStrategy):

    model_pipeline: Pipeline
    past_data: DataFrame

    def __init__(
        self,
        model_pipeline: Pipeline,
        model_request: ModelRequestDTO,
        past_data: DataFrame,
        lag_handler: ILagHandler = None,
    ):
        self.model_pipeline = model_pipeline
        self.past_data = past_data
        self.model_request = model_request
        self.lag_handler = lag_handler

    def execute(self, input_data: DataFrame):
        og_data = self.past_data.select(input_data.columns)
        feature_columns = [
            col.ColumnName for col in self.model_request.ModelConfig.Features
        ]
        predict_df = pl.DataFrame()
        prefix = "_lag"

        for index in range(0, len(input_data)):
            # The row based on index
            row = input_data[index]
            # Append row to original dataset.
            og_data = og_data.vstack(row)
            logging.debug(og_data.head(6))
            # Apply lagged changes.
            lagged_data = self.lag_handler.execute_change(og_data, feature_columns)
            # Lagged data causes records with no value. So this will be deleted.
            lagged_data = lagged_data.fill_null(strategy="backward")
            # Pick and delete the last element of the slice
            last_df = lagged_data.slice(-1, 1)
            # Remove the first element
            og_data = og_data[:-2]
            logging.debug(og_data)
            lag_features = [col for col in last_df.columns if col.__contains__(prefix)]
            # Select timestamp and lag_features
            last_df = last_df.select([TIMESTAMP] + lag_features)
            predict_df = pl.concat([predict_df, last_df])
            # Put the last element to the first. Cycling through each record one by one.
            og_data = pl.concat([og_data.tail(1), og_data])
        # Use the preprocessed prediction data to predict a forecast.
        prediction_result = self.model_pipeline.predict(predict_df)
        logging.debug(prediction_result)
        # Use the model pipeline
        return prediction_result

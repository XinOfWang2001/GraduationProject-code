import logging

from polars import DataFrame
from sklearn.pipeline import Pipeline

from Domain import ModelRequest
from services.AbstractServices import ForecastPredictorStrategy, ILagHandler

from .MultiFeaturePredictor import MultiFeaturePredictor
from .UnivariatePredictor import UnivariatePredictor


class PredictorStrategyPicker:

    def __init__(self, lag_handler: ILagHandler):
        self.lag_handler = lag_handler

    def choose_predictor(
        self,
        model_request: ModelRequest,
        model_pipeline: Pipeline,
        past_data: DataFrame = None,
    ) -> ForecastPredictorStrategy:
        """
        Based on modelrequest it should pick the right method.
        - Features == 0
        - Features > 0 and past_data is not None, select multi feature strategy
        - Otherwise return Exception
        """

        if len(model_request.ModelConfig.Features) > 0 and past_data is not None:
            return MultiFeaturePredictor(
                model_pipeline, model_request, past_data, self.lag_handler
            )
        if len(model_request.ModelConfig.Features) == 0:
            logging.debug("Univariate chose by the PICKER")
            return UnivariatePredictor(model_pipeline)

        raise ValueError(
            "Based on the parameters provided, no predictor could be chosen."
        )

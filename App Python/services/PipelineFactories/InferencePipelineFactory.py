from services.AbstractServices import InferencePipelineTemplate
from services.ImplServices import (CalculationBuilder,
                                   ForecastInferencePipeline, LagHandler,
                                   PredictorStrategyPicker)

from .DataPipelineFactory import DataPipelineFactory


class InferencePipelineFactory:
    """
    Responsible for the creation of inference pipeline classes.
    """

    def __init__(self, data_pipeline_factory: DataPipelineFactory):
        self.data_pipeline_factory = data_pipeline_factory
        self.lag_handler = LagHandler()
        self.predictor_picker = PredictorStrategyPicker(self.lag_handler)
        self.calculation_builder = CalculationBuilder()

    def create_forecast_inference_pipeline(
        self, existing_pipeline=None
    ) -> InferencePipelineTemplate:
        """Responsible for the creation of the forecast inference pipeline,
        including all the necessary services."""
        data_pipeline = self.data_pipeline_factory.create_fulldata_pipeline()
        # Future: Add repository to inference pipeline and check.
        return ForecastInferencePipeline(
            self.calculation_builder,
            model_pipeline=existing_pipeline,
            data_pipeline=data_pipeline,
            lag_handler=self.lag_handler,
            predictor_picker=self.predictor_picker,
        )

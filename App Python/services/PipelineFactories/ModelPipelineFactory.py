import logging

from services.AbstractServices import ModelPipelineTemplate
from services.ImplServices import (CalculationBuilder,
                                   ForecastingTrainingPipeline, LagHandler,
                                   PipelineBuilder)
from services.PipelineFactories import DataPipelineFactory


class ModelPipelineFactory:
    """
    This will be responsible for creating modelpipelines.
    Every pipeline will be fitted with a data pipeline, to collect data into it.
    """

    def __init__(self, data_pipeline_factory: DataPipelineFactory):
        self.data_pipeline_factory = data_pipeline_factory
        self.lag_handler = LagHandler()

    def create_forecasting_pipeline(self) -> ModelPipelineTemplate:
        # Create full data pipeline
        full_data_pipeline = self.data_pipeline_factory.create_fulldata_pipeline()
        # Create pipeline builder
        pipeline_builder = PipelineBuilder()
        calculation_builder = CalculationBuilder()
        logging.debug("Creating full data pipeline")
        return ForecastingTrainingPipeline(
            full_data_pipeline, pipeline_builder, calculation_builder, lag_handler=self.lag_handler
        )

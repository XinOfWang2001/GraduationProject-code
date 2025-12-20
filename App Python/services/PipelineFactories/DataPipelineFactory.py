import logging

from polars import DataFrame

from ExternalServices.ExternalServiceFacade import ExternalServiceFacade
from services.AbstractServices import DataPipelineTemplate
from services.ImplServices import (DataTransformer, FullDataPipeline,
                                   InMemoryDataPipeline, PreviewDataPipeline)


class DataPipelineFactory:
    """This class is responsible for creating datapipelines."""

    external_service_facade: ExternalServiceFacade
    data_transformer: DataTransformer

    def __init__(self, facade):
        self.external_service_facade = facade
        self.data_transformer = DataTransformer()

    def create_preview_pipeline(self) -> DataPipelineTemplate:
        """Creates a preview data pipeline"""
        logging.debug("Preview data pipeline has been chosen")
        return PreviewDataPipeline(self.data_transformer, self.external_service_facade)

    def create_fulldata_pipeline(self) -> DataPipelineTemplate:
        """Creates a full datapipeline"""
        logging.debug("Full data pipeline chosen")
        return FullDataPipeline(self.data_transformer, self.external_service_facade)

    def create_in_memory_pipeline(self, df: DataFrame):
        """Creates a in memory datapipeline. Only being used to reduce roundtrips to the API."""
        return InMemoryDataPipeline(existing_data=df)

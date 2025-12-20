"""Contains the Preview datapipeline class. Inheriting the DataPipelineTemplate class."""
import logging

from polars import DataFrame

from services.AbstractServices import DataPipelineTemplate

from ..DataPreprocessing.Transformers.PivotTransformer import DataTransformer


class PreviewDataPipeline(DataPipelineTemplate):
    """
    An implementation of the datapipeline template that only retrieves per timestamp 40 records

    This is only meant to preview data from the API's
    """
    data_transformer: DataTransformer

    def __init__(self, data_transformer: DataTransformer, external_service=None):
        self.data_transformer = data_transformer
        super().__init__(external_service)

    async def execute(self, data_request):
        """
        Will execute the extract, transform and load functions,
        in that order by calling the Execute function.
        """
        logging.info("execute pipeline")
        return await super().execute(data_request)

    async def _extract(self, data_request):
        """
        Will retrieve data from data source.
        """
        logging.debug("Get PREVIEW - data")
        # In the preview datapipelines, the amount of points is overridden to 5.
        data_request.Points = 5
        data = await self.external_service.use_service(data_request)
        return data

    def _transform(self, data_request):
        """Converts data into pivottables"""
        logging.debug("TRANSFORM Preview data")
        if data_request.is_empty():
            return data_request
        logging.debug(data_request)
        pivot_table: DataFrame = self.data_transformer.transform_into_pivot_table(data_request)
        pivot_table = pivot_table.drop_nulls()
        return pivot_table

    def _load(self, data_request):
        """
        Loads data to a file or datasource.
        """
        logging.info("Load data")
        return data_request

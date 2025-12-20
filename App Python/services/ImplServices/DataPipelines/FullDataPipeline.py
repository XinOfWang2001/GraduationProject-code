"""
This file contains the implementation of the FullDataPipeline.
This will retrieve data, based on the 'Amount of Data' parameter provided by the client
It inherits the DataPipeline Template abstract class.
"""
import logging

from polars import DataFrame

from services.AbstractServices import DataPipelineTemplate

from ..DataPreprocessing.Transformers.PivotTransformer import DataTransformer


class FullDataPipeline(DataPipelineTemplate):
    """
    An implementation of the datapipeline template that retrieves 
    as much data as much as the client requests.
    """

    def __init__(self, data_transformer: DataTransformer, external_service=None):
        self.data_transformer = data_transformer
        super().__init__(external_service)

    async def execute(self, data_request):
        return await super().execute(data_request)

    async def _extract(self, data_request):
        return await self.external_service.use_service(data_request)

    def _transform(self, data_request: DataFrame):
        if data_request.is_empty():
            return data_request
        pivot_table = self.data_transformer.transform_into_pivot_table(data_request)
        pivot_table = pivot_table.interpolate()
        # In case null values are still present at the last and first elements, delete them.
        pivot_table = pivot_table.drop_nulls()
        return pivot_table

    def _load(self, data_request):
        logging.debug(data_request.head(3))
        return data_request

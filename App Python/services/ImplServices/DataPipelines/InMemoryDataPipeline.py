from polars import DataFrame

from services.AbstractServices import DataPipelineTemplate


class InMemoryDataPipeline(DataPipelineTemplate):

    def __init__(self, external_service=None, existing_data: DataFrame = None):
        self.existing_data = existing_data
        super().__init__(external_service)

    async def execute(self, data_request):
        return await super().execute(data_request)

    async def _extract(self, data_request):
        return self.existing_data

    def _transform(self, data_request):
        return self.existing_data

    def _load(self, data_request):
        return self.existing_data

import logging
from abc import ABC, abstractmethod

from polars import DataFrame

from Domain import DataRequest
from ExternalServices import ExternalServiceFacade


class DataPipelineTemplate(ABC):
    """
    This provides the template of the ETL processes
    Extract
    Transform
    Load
    """

    external_service: ExternalServiceFacade

    # Is an optional field, because not every pipeline needs a external source.
    def __init__(self, external_service: ExternalServiceFacade = None):
        self.external_service = external_service
        super().__init__()

    @abstractmethod
    async def execute(self, data_request: DataRequest) -> DataFrame:
        """Executes all of the methods in a sequence"""
        data = await self._extract(data_request=data_request)
        logging.info(data.head())
        transformed = self._transform(data)
        loaded = self._load(transformed)
        logging.info(loaded.head())
        return loaded

    # Represent private methods.
    @abstractmethod
    async def _extract(self, data_request: DataRequest) -> DataFrame:
        """Retrieves data from data source"""

    @abstractmethod
    def _transform(self, data_request: DataFrame) -> DataFrame:
        """Transforms data"""

    @abstractmethod
    def _load(self, data_request: DataFrame) -> DataFrame:
        """Loads data"""

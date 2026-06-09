"""Module for hosting the ExternalServiceFacade class"""
import logging

from Domain import DataRequest

from services.AbstractServices import AbstractExternalService

class ExternalServiceFacade:
    """
    Decides which external service will be called upon
    - If Datasource is WEBAPI --> Use Web API
    - If Datasource is IoTHub --> Use IoT API
    - Else exception should be thrown
    - After data is retrieved succesfull, return DataFrame
    """

    # Initialize external API services.
    WEBAPI_api: AbstractExternalService

    def __init__(self, WEBAPI: AbstractExternalService):
        self.WEBAPI_api = WEBAPI

    async def use_service(self, dto: DataRequest):
        """Should decide if Web or another service need to be called upon.
        For now only the Web data source is used."""
        logging.debug("Choose Web Service")
        return await self.WEBAPI_api.return_value(dto)

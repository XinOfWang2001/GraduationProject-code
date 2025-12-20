import asyncio
import logging

import httpx
import polars as pl
from polars import DataFrame, json_normalize

from const import DATA_FORMAT, FORMAT_API, TIMESTAMP
from Domain import DataRequest
from services.AbstractServices import AbstractExternalService


class APIService(AbstractExternalService):
    """description of class"""

    base_url: str
    target_url: str
    format: str

    def __init__(self, base: str, target: str):
        self.base_url = base
        self.target_url = target
        self.format = DATA_FORMAT

    async def return_value(self, request: DataRequest):
        """
        This method will perform a API request to the WEBAPI-API.
        - It will send the request
        - Parse the JSON to Polars Dataframe.
        """
        logging.info("Start request WEBAPI")
        # Format dto to GET: request url
        request_api: str = self._convert_dto_into_url(request)
        # Call API.
        task = asyncio.create_task(self._get_data(request_api))
        response = await task
        # Parse data into an dataframe.
        logging.info("Parse data collected")
        value = json_normalize(response)
        logging.debug("Start parsing values to datetime")
        value.head(2)
        # Convert datetime string to datetime format
        value = self._convert_to_datetime(value)
        # Return dataframe
        return value

    def _convert_dto_into_url(self, dto: DataRequest) -> str:
        """
        Converts the request body into a workable url.
        """
        obs_concat: str = self._convert_int_list_to_string(dto.ObservationIds)
        vt_concat: str = self._convert_int_list_to_string(dto.ValueTypeIds)
        input_url = FORMAT_API.format(
            token=dto.Token,
            st=dto.StartDateUnix,
            et=dto.EndDateUnix,
            obs=obs_concat,
            vts=vt_concat,
            points=dto.Points,
            format=self.format,
            tn=dto.Timelevel,
        )
        request_string: str = (
            f"{self.base_url}{dto.Project}{self.target_url}{input_url}"
        )
        return request_string

    def _convert_int_list_to_string(self, input_list: list[int]):
        delimiter = ","
        return delimiter.join(map(str, input_list))

    def _convert_to_datetime(self, data: DataFrame):
        return data.with_columns(
            pl.col(TIMESTAMP).str.to_datetime(time_zone="UTC")
        )

    async def _get_data(self, url: str):
        async with httpx.AsyncClient() as client:
            result = await client.get(url, timeout=30.0)
            return result.json()  # Assuming you want to return JSON data

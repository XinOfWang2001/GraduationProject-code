import logging
import os
import traceback

from fastapi import APIRouter, HTTPException

from Domain import DataRequestDTO, DataResponseDTO
from ExternalServices import ExternalServiceFacade, APIService
from services.AbstractServices import DataPipelineTemplate
from services.ImplServices import DTODomainMapper, DTOResponseMapper
from services.PipelineFactories import DataPipelineFactory

router = APIRouter()
BASE_URL = os.getenv("BASE_URL_WEBAPI")
TARGET_URL = os.getenv("TARGET_URL")

WEBAPI_service = APIService(BASE_URL, TARGET_URL)
external_service = ExternalServiceFacade(WEBAPI_service)
data_pipeline_factory = DataPipelineFactory(external_service)
data_request_mapper = DTOResponseMapper()
dto_domain_mapper = DTODomainMapper()


@router.post("/data-operation/preview")
async def get_columns(data_request: DataRequestDTO):
    try:
        logging.debug(data_request.model_dump_json())
        data_domain = dto_domain_mapper.map_datarequest_dto_to_domain(data_request)
        # Create preview pipeline
        preview_data_pipeline: DataPipelineTemplate = (
            data_pipeline_factory.create_preview_pipeline()
        )
        # Use pipeline
        result = await preview_data_pipeline.execute(data_domain)

        # Return result in a proper format!
        response_dto: DataResponseDTO = data_request_mapper.map_to_data_response_dto(
            result, data_domain.ProvideData
        )
        logging.info("Retrieving preview data has succeeded")
        # Map results to a DTO.
        return response_dto
    except Exception as inst:
        traceback.print_exc()
        logging.error("Retrieving preview data has failed, attention is needed.")
        raise HTTPException(400, detail=inst.args)

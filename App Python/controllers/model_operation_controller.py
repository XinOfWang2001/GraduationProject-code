import logging
import os
import traceback

from dotenv import load_dotenv
from fastapi import APIRouter, HTTPException

from Domain import (ModelForecastRequestDTO, ModelRequestDTO,
                    ModelStorageCreationDTO)
from ExternalServices import (ExternalServiceFacade, APIService,
                              ModelRepository)
from services.ImplServices import DTODomainMapper, DTOResponseMapper
from services.PipelineFactories import (DataPipelineFactory,
                                        InferencePipelineFactory,
                                        ModelPipelineFactory)

load_dotenv()

CHECK_ENV = os.getenv("CHECK_ENV", "== Niets ingesteld ==")
BASE_URL = os.getenv("BASE_URL")
TARGET_URL = os.getenv("TARGET_URL")
ACCOUNT_URL = os.getenv("ACCOUNT_URL")
CONTAINER_NAME = os.getenv("CONTAINER_NAME", "development")
BLOB_CONN_STR = os.getenv("BLOB_CONN_STR")

# Services
WEBAPI_service = APIService(BASE_URL, TARGET_URL)
external_service = ExternalServiceFacade(WEBAPI_service)
model_repository = ModelRepository(
    container_name=CONTAINER_NAME, connection_str=BLOB_CONN_STR
)
data_pipeline_factory = DataPipelineFactory(external_service)
modeling_pipeline_factory = ModelPipelineFactory(data_pipeline_factory)
inference_pipeline_factory = InferencePipelineFactory(data_pipeline_factory)
data_request_mapper = DTOResponseMapper()
dto_domain_mapper = DTODomainMapper()

router = APIRouter()

@router.post("/model-operation/preview")
async def train_model_preview(model_request_dto: ModelRequestDTO):
    try:
        print(model_request_dto.model_dump_json())
        logging.info(model_request_dto.model_dump_json())
        # Create model pipeline
        model_pipeline = modeling_pipeline_factory.create_forecasting_pipeline()
        # Map DTO to Domain
        model_request = dto_domain_mapper.map_modeltraining_request_to_domain(
            model_request_dto
        )
        # Train model
        results = await model_pipeline.execute(model_request)
        # Set prediction parameters with current data.
        model_request.PredictionParameters = (
            dto_domain_mapper.map_to_model_forecast_params(
                model_request.DataRequest.get_end_date(),
                model_request.ModelConfig.ForecastingDate,
            )
        )
        logging.info("Model training successfull")
        # Immediately use the model to perform inference task.
        # Later will be replaced with a generic method of creating inference pipelines.
        inference_pipeline = (
            inference_pipeline_factory.create_forecast_inference_pipeline(
                existing_pipeline=results.model_pipeline
            )
        )
        prediction_result = await inference_pipeline.execute(model_request)
        # For forecasting purposes, only data whereby forecasts has run, will be returned.
        # For other models this should be handled differently.
        results.data = results.get_original_data(
            [col.ColumnName for col in model_request.ModelConfig.Targets]
        )
        results.set_forecasted_data(prediction_result.get_predicted_data())
        # Map results to format.
        response_dto = data_request_mapper.map_to_model_result(results)
        # Use models
        return response_dto
    except:
        traceback.print_exc()
        logging.error("Model training has failed. It needs attention.")
        raise HTTPException(400, detail="An Error has ocurred")

@router.post("/model-operation/model-storage")
async def store_model(model_creation_request: ModelStorageCreationDTO):
    """
    This endpoints has the following responsibilities:
    - Training the model
    - Persisting the model

    Return values:
    - A DTO with the model-location, address
    """
    try:
        logging.debug(model_creation_request.model_dump_json())
        # Create model pipeline
        model_pipeline = modeling_pipeline_factory.create_forecasting_pipeline()
        model_request = dto_domain_mapper.map_modelstorage_request_to_domain(
            model_creation_request
        )
        model_result = await model_pipeline.execute(modeling_request=model_request)
        logging.info("Start model persistence.")
        model_location = model_repository.create(model_result)
        logging.info("Model has succesfully been stored")
        response = data_request_mapper.map_to_model_storage(
            model_location, model_result
        )
        return response
    except:
        traceback.print_exc()
        logging.error("Model storage has failed. Please take a look.")
        raise HTTPException(400, detail="Something went wrong with the model storage")

@router.post("/model-operation/forecast")
async def generate_predictions(prediction_request: ModelForecastRequestDTO):
    try:
        logging.debug(prediction_request.model_dump_json())
        model_request = dto_domain_mapper.map_prediction_request_to_domain(
            prediction_request
        )
        # Get model based on guid and version
        model_pipeline = model_repository.get_model(model_request.ModelLocation)
        # Check if model exists
        if model_pipeline is None:
            return HTTPException(404, detail="Model not found")
        inference_pipeline = inference_pipeline_factory.create_forecast_inference_pipeline(existing_pipeline=model_pipeline)
        model_request.correct_data_request_dates()
        result = await inference_pipeline.execute(model_request)
        # Map modeltrainingsresult to endresult
        response_dto = data_request_mapper.map_to_prediction_result(result)
        return response_dto
    except:
        traceback.print_exc()
        logging.error("Generating predictions has failed. Please investigate.")
        return HTTPException(
            400, detail="Generating predictions has failed, please investigate"
        )

@router.delete("/model-operation/remove/{name_model}")
async def delete_model(name_model: str):
    try:

        logging.info(f"Attempt to delete model with the name {name_model} has started")
        result = model_repository.delete_model(name_model)
        logging.info(result.message)
    except:
        traceback.print_exc()
        logging.error(f"Model has {name_model} either been deleted, does not exist or connection broke.")
    # Should return true, because the delete function should be idempotent, regardless if it has deleted a file or not.
    return True
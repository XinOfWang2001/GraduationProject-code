from datetime import datetime, timezone

from pydantic import BaseModel

from Domain import DataRequestDTO, ModelAlgorithm, ModelConfigDTO, ModelType

from .CalculationStepDTO import CalculationStepDTO


class ModelStorageCreationDTO(BaseModel):
    DateOfAction: datetime
    DataRequest: DataRequestDTO
    ModelConfig: ModelConfigDTO
    OperationList: list[CalculationStepDTO] = []
    

class ModelStorageDTO(BaseModel):
    """
    This DTO will be used as a response object to the client
    that contains all of the meta data
    """
    WorkspaceGuid: str 
    ModelAddress: str 
    ModelName: str 
    ModelVersion: str = "latest"
    ModelType: ModelType
    ModelAlgorithm: ModelAlgorithm
    DateOfCreation: datetime = datetime.now(timezone.utc)
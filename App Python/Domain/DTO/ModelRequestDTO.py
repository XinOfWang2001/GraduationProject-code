from datetime import datetime

from pydantic import BaseModel

from Domain import DataRequestDTO, ModelConfigDTO

from .CalculationStepDTO import CalculationStepDTO


class ModelRequestDTO(BaseModel):
    DateOfAction: datetime
    DataRequest: DataRequestDTO
    ModelConfig: ModelConfigDTO
    OperationList: list[CalculationStepDTO] = []

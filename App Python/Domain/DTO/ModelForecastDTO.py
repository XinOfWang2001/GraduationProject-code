from datetime import datetime

from pydantic import BaseModel

from .CalculationStepDTO import CalculationStepDTO
from .DataRequestDTO import DataRequestDTO
from .ModelConfigDTO import ModelConfigDTO


class ModelTimeForecastDTO(BaseModel):
    CurrentDate: datetime
    # Is stored in nanoseconds
    PeriodsInAdvance: float
    # Variable of how much forward new dates should be generated. Is used to calculate the future date
    # Currentdate, point to which new dates will be created


class ModelForecastRequestDTO(BaseModel):
    DataRequest: DataRequestDTO
    ModelConfig: ModelConfigDTO
    OperationList: list[CalculationStepDTO] = []
    ModelPredictionParameters: ModelTimeForecastDTO
    ModelStorageAddress: str

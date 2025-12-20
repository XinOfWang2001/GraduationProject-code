from datetime import datetime

from pydantic import BaseModel

from Domain import (AlgorithmUnion, DataColumnDTO, DateTimeLevel,
                    ModelAlgorithm, ModelType)


class ModelConfigDTO(BaseModel):
    ModelConfigGuid: str
    ParentWorkspaceGuid: str
    ModelName: str
    DataSplitRatio: float
    ForecastingDate: datetime
    DateTimeLevel: DateTimeLevel
    ModelType: ModelType
    ModelAlgorithm: ModelAlgorithm
    Features: list[DataColumnDTO] = []
    Targets: list[DataColumnDTO] = []
    AlgorithmParameterDTO: AlgorithmUnion

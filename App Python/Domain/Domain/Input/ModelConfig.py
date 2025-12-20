from datetime import datetime

from pydantic import BaseModel

from ..Enums.Enums import DateTimeLevel, ModelAlgorithm, ModelType
from .DataColumn import DataColumn
from .ModelParameters import ParamUnion


class ModelConfig(BaseModel):
    ModelConfigGuid: str
    ParentWorkspaceGuid: str
    ModelName: str
    DataSplitRatio: float
    ForecastingDate: datetime
    DateTimeLevel: DateTimeLevel
    ModelType: ModelType
    ModelAlgorithm: ModelAlgorithm
    Features: list[DataColumn] = []
    Targets: list[DataColumn] = []
    ModelParameter: ParamUnion

    def get_targets(self) -> list[str]:
        return [Target.ColumnName for Target in self.Targets]
    
    def get_test_percentage(self):
        return (1 - self.DataSplitRatio)
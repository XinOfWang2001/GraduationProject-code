from typing import Union

from pydantic import BaseModel

from ..Enums.Enums import CalculationType


class Calculation(BaseModel):
    CalculationId: int = 0
    OutputColumn: str


class DynamicKPI(Calculation):
    CalculationString: str
    OperationList: list[str]

CalculationUnion = Union[DynamicKPI]

class CalculationStep(BaseModel):
    Order: int = 0
    CalculationType: CalculationType
    Operations: list[CalculationUnion]

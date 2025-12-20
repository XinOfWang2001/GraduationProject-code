from pydantic import BaseModel

from Domain import CalculationType

from .CalculationDTO import CalculationDTOUnion


class CalculationStepDTO(BaseModel):
    Order: int 
    CalculationType: CalculationType
    Calculations: list[CalculationDTOUnion]
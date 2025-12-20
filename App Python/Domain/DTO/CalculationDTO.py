from typing import Union

from pydantic import BaseModel


class CalculationDTO(BaseModel):
    InputColumns: list[str] = []
    OutputColumn: str

class KPIDTO(CalculationDTO):
    CalculationString: str 
    OperationsList: list[str]

# class AggregationDTO(CalculationDTO):
#     Action: str 

CalculationDTOUnion = Union[KPIDTO]



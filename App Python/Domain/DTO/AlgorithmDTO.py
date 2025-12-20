from typing import Union

from pydantic import BaseModel


class AlgorithmDTO(BaseModel):
    Id: int
    TypeOfAlgorithm: str


class LinearRegressionDTO(AlgorithmDTO):
    NJobs: int


class SVMDTO(AlgorithmDTO):
    Kernel: str


AlgorithmUnion = Union[LinearRegressionDTO, SVMDTO]

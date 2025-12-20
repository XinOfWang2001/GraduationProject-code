from typing import Union

from pydantic import BaseModel


class ModelParams(BaseModel):
    Id: int
    TypeOfAlgorithm: str


class LinearRegressionParam(ModelParams):
    NJobs: int


class SVMParam(ModelParams):
    Kernel: str


ParamUnion = Union[LinearRegressionParam, SVMParam]
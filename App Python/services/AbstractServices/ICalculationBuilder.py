from abc import ABC, abstractmethod
from typing import Any, Self

from Domain import ModelRequest

from .ICalculationHandler import ICalculationHandler


class ICalculationBuilder(ABC):

    @abstractmethod
    def reset(self):
        pass 

    @abstractmethod
    def add_calculations(self, model_request: ModelRequest) -> Self:
        pass

    @abstractmethod
    def build(self) -> ICalculationHandler:
        pass
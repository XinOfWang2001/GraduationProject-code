from abc import ABC, abstractmethod

from polars import DataFrame


class ICalculationHandler(ABC):

    @abstractmethod
    def execute(self, data: DataFrame) -> DataFrame:
        """
        Performs data transformation based on the provided methods.
        An succesfull outcome is either a transformed data or an unchanged one.
        If it is invalid, it should throw an exception
        """
        pass
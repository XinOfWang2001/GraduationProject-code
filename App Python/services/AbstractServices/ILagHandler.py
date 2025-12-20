from abc import ABC, abstractmethod

from polars import DataFrame


class ILagHandler(ABC):

    @abstractmethod
    def execute_change(self, df: DataFrame, input_columns: list) -> DataFrame:
        """
        This method will perform a lag operation based on implementation.
        """

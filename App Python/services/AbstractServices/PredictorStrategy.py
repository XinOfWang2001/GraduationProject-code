from abc import ABC, abstractmethod

from polars import DataFrame


class ForecastPredictorStrategy(ABC):

    @abstractmethod
    def execute(self, input_data: DataFrame) -> DataFrame:
        """
        This is the main entry point of any Forecast Predictor Strategy class.
        Its endresult will be unprocessed model result data,
        formatted in ndarray (Numpy) list<ndarray> [[y_result1, y_result2]]
        """

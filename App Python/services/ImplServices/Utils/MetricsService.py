from polars import DataFrame
from sklearn.metrics import (mean_absolute_percentage_error,
                             root_mean_squared_error)


class MetricsService:
    """
    A wrapper class to encapsulate all of the metrics related methods of sklearn or related.
    """

    def __init__(self):
        pass

    def generate_rmse(self, original: DataFrame, predicted: DataFrame) -> float:
        return round(root_mean_squared_error(original, predicted), 5)

    def generate_mape(self, original: DataFrame, predicted: DataFrame) -> float:
        return round(mean_absolute_percentage_error(original, predicted) * 100, 3) 

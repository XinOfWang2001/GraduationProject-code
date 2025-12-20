from abc import ABC, abstractmethod

from polars import DataFrame

from Domain import ModelConfig


class ModelResult(ABC):
    data: DataFrame
    forecasted_data: DataFrame
    model_config: ModelConfig

    def __init__(self, data, config: ModelConfig):
        self.data = data
        self.model_config = config
        self.forecasted_data = DataFrame()

    @abstractmethod
    def get_original_data(self, selected_columns: list[str]):
        """Returns the orginal dataset to the client"""

    @abstractmethod
    def set_forecasted_data(self, data: DataFrame):
        """Assigns prediction data to the result"""

    @abstractmethod
    def get_predicted_data(self) -> DataFrame:
        """"Returns prediction data"""


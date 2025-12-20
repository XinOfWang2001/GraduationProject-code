from polars import DataFrame

from const import TIMESTAMP
from Domain import ModelConfig

from .ModelResult import ModelResult


class PredictionResult(ModelResult):
    data: DataFrame
    forecasted_data: DataFrame
    model_config: ModelConfig

    def __init__(self, data: DataFrame, predicted_data: DataFrame, config: ModelConfig):
        super().__init__(data, config)
        self.forecasted_data = predicted_data

    def get_original_data(self, selected_columns: list[str]):
        return self.data.select([TIMESTAMP] + [col for col in selected_columns])

    def get_full_target_data(self):
        targets = [col.ColumnName for col in self.model_config.Targets]
        return self.data.select([TIMESTAMP] + targets)

    def get_predicted_data(self):
        return self.forecasted_data
    
    def set_forecasted_data(self, data):
        return super().set_forecasted_data(data)

from polars import DataFrame, concat
from sklearn.pipeline import Pipeline

from const import TIMESTAMP
from Domain import ModelConfig

from .ModelResult import ModelResult


class TrainingFeatureClass:
    features: DataFrame = DataFrame()
    targets: DataFrame = DataFrame()
    predictions: DataFrame

    def __init__(self, features: DataFrame, targets: DataFrame):
        self.features = features
        self.targets = targets

    def add_predictions(self, data: DataFrame):
        self.predictions = data


class Metric:
    metric_name: str
    column_name: str
    value: float

    def __init__(self, name, column_name, value):
        self.metric_name = name
        self.value = value
        self.column_name = column_name


class ModelTrainingsResult(ModelResult):
    model_pipeline: Pipeline
    model_metrics: list[Metric]

    training_set: TrainingFeatureClass
    test_set: TrainingFeatureClass

    def __init__(self, data: DataFrame, model_pipeline: Pipeline, config: ModelConfig):
        super().__init__(data, config)
        self.model_pipeline = model_pipeline
        self.model_metrics = []

    def add_metric(self, metric: str, name: str, value: float):
        self.model_metrics.append(Metric(metric, name, value))

    def get_original_data(self, selected_columns: list[str]):
        return self.data.select([TIMESTAMP] + [col for col in selected_columns])

    def set_trainings_set(self, X: DataFrame, Y: DataFrame):
        self.training_set = TrainingFeatureClass(X, Y)

    def set_test_set(self, X: DataFrame, Y: DataFrame):
        self.test_set = TrainingFeatureClass(X, Y)

    def set_forecasted_data(self, data: DataFrame):
        self.forecasted_data = data

    def get_full_feature_data(self):
        return concat([self.training_set.features, self.test_set.features])
    
    def get_full_target_data(self):
        return concat([self.training_set.targets, self.test_set.targets])
    
    def get_predicted_data(self):
        return concat([self.test_set.predictions, self.forecasted_data])

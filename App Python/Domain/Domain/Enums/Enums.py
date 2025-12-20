from enum import Enum


class ModelType(Enum):
    FORECASTING = 0
    OUTLIER_DETECTION = 1


class ModelAlgorithm(Enum):
    LINEAR_REGRESSION = 0
    SVMREGRESSION = 1


class DateTimeLevel(Enum):
    STANDARD = 0
    ONLY_DATES = 1

class CalculationType(Enum):
    KPI = 0
    AGGREGATION = 1
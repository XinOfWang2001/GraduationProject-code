from pydantic import BaseModel

from ..Domain.ModelResult.DataSeries import DataSeries


class DataColumnDTO(BaseModel):
    """This DTO will encapsulate the column and its datatype"""

    ColumnName: str
    DataType: str

class MetricsDTO(BaseModel):
    Metric: str
    Column: str
    Value: float

class DataResponseDTO:
    """
    This class will hold all of the data necessary to be send back to the client
    Required response:
    - Columnnames + Datatypes
    - Amount of data per column

    Optional
    - The dataset in formatted in dataframe/JSON array. (Only if requested)
    """

    DataColumns: list[DataColumnDTO]
    DataCount: int

    DataSet: DataSeries | None

    def __init__(
        self, columnnames: list[DataColumnDTO], data_count: int, dataset: DataSeries
    ):
        self.DataColumns = columnnames
        self.DataCount = data_count
        self.DataSet = dataset

class PredictionResultDTO(DataResponseDTO):
    PredictionSet: DataSeries

    def __init__(self, columnnames, data_count, dataset, predicted_data: DataSeries):
        self.PredictionSet = predicted_data
        super().__init__(columnnames, data_count, dataset)

class ModelResultDTO(DataResponseDTO):
    # Extra dataset for test and training set should be inserted.
    # Perhaps expanded to include the feature set to which predictions are based upon.
    PredictionSet: DataSeries
    MetricsKeyValue: dict[str, list[MetricsDTO]]
    def __init__(
        self, columnnames, data_count, dataset, forecast_dataset: DataSeries, metrics_dict
    ):
        self.PredictionSet = forecast_dataset
        self.MetricsKeyValue = metrics_dict
        super().__init__(columnnames, data_count, dataset)


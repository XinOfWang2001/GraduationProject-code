from abc import ABC, abstractmethod

from polars import DataFrame
from sklearn.pipeline import Pipeline

from Domain import DataRequest, ModelRequest, ModelResult

from .DataPipelineTemplate import DataPipelineTemplate


class InferencePipelineTemplate(ABC):
    data_pipeline: DataPipelineTemplate
    model_pipeline: Pipeline
    current_data: DataFrame
    input_data: DataFrame
    predicted_data: DataFrame

    def __init__(
        self, pipeline: Pipeline = None, data_pipeline: DataPipelineTemplate = None
    ):
        self.model_pipeline = pipeline
        self.data_pipeline = data_pipeline
        self.current_data = DataFrame()

    @abstractmethod
    async def execute(
        self, model_request: ModelRequest, **optional: dict
    ) -> ModelResult:
        """
        This method is responsible for performing inference duties. (Making predictions)
        It should eventually return a DataFrame containing the results.
        """
        self.current_data = await self._load_data(model_request.DataRequest)
        await self._load_model(optional.get("identifier"))
        self.input_data = self._transform(self.current_data, model_request)
        self.predicted_data = self._make_predictions(model_request)
        return self._format_results(model_request)

    @abstractmethod
    async def _load_data(self, data_request: DataRequest):
        """
        This will retrieve data from external sources.
        Loading up data is only necessary when the model utilizes lagged features.
        - It will generate timestamp data.
        Optional step: will be bypassed if no features are selected.
        - Create lagged features.
        """

    @abstractmethod
    async def _load_model(self, identifier: str = None):
        """
        This function is responsible for loading up the model
        If model is already set. return current model_pipeline
        """

    @abstractmethod
    def _transform(self, current_data:DataFrame, model_request: ModelRequest) -> DataFrame:
        """
        This method is responsible for transforming data ready for prediction.
        The output of this method will be assigned to the input_data param
        of this class and used for model training.
        """

    @abstractmethod
    def _make_predictions(self, model_request: ModelRequest) -> DataFrame:
        """
        This function will generate the predictions
        """

    @abstractmethod
    def _format_results(self, model_request: ModelRequest) -> ModelResult:
        """
        This function will be called as last and is meant to format all results
        """

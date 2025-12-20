"""
This is the abstract class of the modelingpipeline template class
The aim of of this class is to standardize all of the steps necessary to train a model.
From data extraction to modeling and perhaps storage.
"""
import logging
from abc import ABC, abstractmethod

from polars import DataFrame

from Domain import DataRequest, ModelConfig, ModelRequest, ModelTrainingsResult


class ModelPipelineTemplate(ABC):
    """
    This template will perform 4 mandatory steps (M) and 1 optional steps. (O)

    Legenda:
    O = Optional
    M = Mandatory

    The following steps in order:
    - Extract data (M) - Can be used to train the model, or be used as reference data
    - Perform standard transformations on data (M) - Standard Timeseries features.
    - Perform user provided transformations. (O) - Like regular calculations or aggregations.
    - Train model (M) - Train model if applicable.
    - Perform post-modeling transformations (M)
    """

    @abstractmethod
    async def execute(self, modeling_request: ModelRequest) -> ModelTrainingsResult:
        data = await self._extract(modeling_request.DataRequest)
        logging.info("Data extraction done")
        data = self._transform(data, modeling_request)
        logging.debug("Added model transformation")
        self._load_model(modeling_request.ModelConfig)
        logging.debug("Loaded model")
        model_result = self._train_model(data, modeling_request.ModelConfig)
        logging.info("Model training done")
        model_result = self._validate_model(model_result)
        logging.info("Post transformation done")
        return model_result

    @abstractmethod
    async def _extract(self, data_request: DataRequest) -> DataFrame:
        """Responsible for extracting data"""

    @abstractmethod
    def _transform(self, df: DataFrame, model_request: ModelRequest) -> DataFrame:
        """This step is responsible for applying transformation or
        configure transformation steps to model pipeline"""

    @abstractmethod
    def _load_model(self, model_config: ModelConfig) -> None:
        """This will execute the machine learning algorithm choice,
        by calling a Algorithm picker class. With the aim of
        picking the right algorithm based on the provided parameters."""

    @abstractmethod
    def _train_model(self, df: DataFrame, model_config: ModelConfig) -> ModelTrainingsResult:
        """
        This method will execute all of the steps provided by the previous four steps.
        It will fit the model of training data. It will return a fitted modelpipeline,
        later used for validation and inference.
        """

    @abstractmethod
    def _validate_model(self, model_result: ModelTrainingsResult) -> ModelTrainingsResult:
        """
        This method will apply post transformations to the data.
        It will perform model validations and apply the metrics
        result to the ModelTrainingsResult class.
        """

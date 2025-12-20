from sklearn.pipeline import Pipeline

from Domain import ModelConfig

from .TransformerFactory import TransformerFactory
from ..DataPreprocessing.Transformers import AlgorithmPicker


class PipelineBuilder:
    """
    This class is responsible for building the Model Pipeline, based on Sklearn.
    The endresult will be a pipeline, containing one modelpipeline.

    Responsibilities:
    - Assigning the Modelalgorithm
    - Assigning mandatory preprocessing steps applied to all columns

    Not:
    - Optional preprocessing steps like lag-creators or KPI or Aggregation Handlers.
    """

    algorithm_picker: AlgorithmPicker
    transformer_factory: TransformerFactory

    inner_number: int = 1
    current_pipeline: Pipeline
    steps: list
    mandatory_features: list

    # Selected parameters
    def __init__(self):
        self.algorithm_picker = AlgorithmPicker()
        self.transformer_factory = TransformerFactory()
        self.mandatory_features = []
        self.reset()

    def reset(self):
        """
        Resets all of the pipeline creation
        """
        self.steps = []
        self.current_pipeline = Pipeline(self.steps)

    # Add Time related feature transformations, based on datetime-level.
    def add_datetime_features(self, modelconfig: ModelConfig):
        transformer = self.transformer_factory.create_date_transformer(modelconfig)
        preprocessing_step = (f"preprocess_step-{self.inner_number}", transformer)
        self.current_pipeline.steps.append(preprocessing_step)
        self._add_step_count()
        return self

    # Apply user based transformations.
    # Assign model based on modeltype.
    def add_model(self, modelconfig: ModelConfig):
        model = self.algorithm_picker.get_machine_learning_algorithm(modelconfig)
        model_step = (f"model_step-{self.inner_number}", model)
        self._add_step_count()
        self.current_pipeline.steps.append(model_step)
        return self

    def build_pipeline(self):
        """
        Builds up pipeline with all of the necessary steps.
        """
        return self.current_pipeline

    def _add_step_count(self):
        self.inner_number += 1

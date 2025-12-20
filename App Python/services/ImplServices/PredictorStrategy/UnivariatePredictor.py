from sklearn.pipeline import Pipeline

from services.AbstractServices import ForecastPredictorStrategy


class UnivariatePredictor(ForecastPredictorStrategy):
    model_pipeline: Pipeline

    def __init__(self, model_pipeline: Pipeline):
        self.model_pipeline = model_pipeline

    def execute(self, input_data):
        return self.model_pipeline.predict(input_data)

from typing import Optional

from pydantic import BaseModel

from .Calculations import Calculation, CalculationStep, CalculationUnion
from .DataRequest import DataRequest
from .ModelConfig import ModelConfig
from .PredictionParameters import ModelPredictParams


class ModelRequest(BaseModel):
    """
    The domain class of the model request
    """
    DataRequest: DataRequest
    ModelConfig: ModelConfig
    Operations: list[CalculationStep] = []
    ModelLocation: Optional[str] = ""
    PredictionParameters: Optional[ModelPredictParams] = None

    def correct_data_request_dates(self):
        """
        This method will correct the start and end unix dates used for proper inference jobs.
        Not applicable when training a model and then generating predictions.
        """
        self.DataRequest.get_corrected_dates()

    def get_upper_predict_date(self):
        """
        Used as upper bound to which date will be predicted 
        """
        return self.PredictionParameters.end_date
    
    def get_lower_bound_date(self):
        """
        Used as lower bound to which date will be predicted.
        Used to determine the startdate of which predictions will be generated. 
        """
        return self.PredictionParameters.current_date
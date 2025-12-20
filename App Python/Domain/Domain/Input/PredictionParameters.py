from datetime import datetime

from pydantic import BaseModel


class ModelPredictParams(BaseModel):
    current_date: datetime 
    end_date: datetime
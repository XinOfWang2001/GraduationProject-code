from pydantic import BaseModel


class DataSeries(BaseModel):
    Timestamps: list 
    Values: dict[str, list] 
    ColumnNames: list 

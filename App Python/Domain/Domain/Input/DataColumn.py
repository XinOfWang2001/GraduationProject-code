from pydantic import BaseModel


class DataColumn(BaseModel):
    """This DTO will encapsulate the column and its datatype"""

    ColumnName: str
    DataType: str
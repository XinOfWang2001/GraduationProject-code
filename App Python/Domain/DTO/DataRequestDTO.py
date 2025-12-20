from typing import Optional

from pydantic import BaseModel


class DataRequestDTO(BaseModel):
    """
    This is the request body needed to retrieve column and preview data.
    The assumption is that an data source is already configured.
    """

    WorkspaceId: str
    Token: str
    DataSource: str
    StartDateUnix: int
    EndDateUnix: int
    Project: str
    Points: int = 20
    Timelevel: Optional[int] = -1
    TimelevelRange: Optional[float] = -1.0
    ObservationIds: list[int]
    ValueTypeIds: list[int]
    ProvideData: bool

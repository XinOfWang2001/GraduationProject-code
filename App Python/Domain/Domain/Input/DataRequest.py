from datetime import datetime
from typing import Optional

from pydantic import BaseModel


class DataRequest(BaseModel):
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
    Timelevel: Optional[int]
    TimelevelRange: Optional[float] = 432000000000.0  # Default 12 Hours
    ObservationIds: list[int]
    ValueTypeIds: list[int]
    ProvideData: bool

    def get_end_date(self):
        return datetime.fromtimestamp(float(self.EndDateUnix) / 1000)

    def get_corrected_dates(self, current_date: datetime = datetime.now()):
        """
        The unix dates need to be corrected when using a timeseries model, because the data it retrieves is used for 
        creating lag-features and requires the most up-to-date data.
        Not correcting during runtime would cause a gap in dates between original data and predicted data.
        It will be only used at just inference. Modeltraining functions should not use this method or any methods related to this.
        """
        unix_current = (int)(current_date.timestamp() * 1000)
        difference = unix_current - self.EndDateUnix
        corrected_enddate_unix = unix_current
        corrected_startdate_unix = self.StartDateUnix + difference
        # Change correction.
        self.StartDateUnix = corrected_startdate_unix
        self.EndDateUnix = corrected_enddate_unix

        print(self.get_end_date())
        print(datetime.fromtimestamp(float(self.StartDateUnix) / 1000))
        return (corrected_startdate_unix, corrected_enddate_unix)

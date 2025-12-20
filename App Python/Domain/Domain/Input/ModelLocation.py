from pydantic import BaseModel


class ModelLocation(BaseModel):
    """
    This is the domain model for model location
    """
    WorkspaceGuid: str 
    ModelAddress: str 
    ModelName: str 
    ModelVersion: str = "latest"
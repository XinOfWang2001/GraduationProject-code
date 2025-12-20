from pydantic import BaseModel

class DeleteDTO(BaseModel):
    message: str 
    has_deleted: bool
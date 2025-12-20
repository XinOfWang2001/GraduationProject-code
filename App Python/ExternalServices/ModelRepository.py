import logging
import traceback
from pickle import dumps, loads

from azure.storage.blob import BlobServiceClient, ContainerClient
from sklearn.pipeline import Pipeline

from const import FORMAT_MODEL_ID
from Domain import ModelLocation, ModelTrainingsResult, DeleteDTO


class ModelRepository:
    """
    This class is responsible for persistence and retrieval of Machine Learning models.
    Tasks:
    - Creation
    - Retrieval by address

    This implemenation will have store models to Azure Blob Storage.
    """

    account_url: str
    container_name: str
    connection_string: str
    blob_client: BlobServiceClient
    container_client: ContainerClient

    def __init__(self, container_name: str, connection_str: str):
        # Create the BlobServiceClient object
        self.container_name = container_name
        self.connection_string = connection_str
        self.blob_client = None

    def load_account(self) -> ContainerClient:
        """
        This method will be executed at launch of the API to connect to Azure Blob storage.
        """
        try:
            if self.blob_client is None:
                self.blob_client = BlobServiceClient.from_connection_string(
                    self.connection_string
                )
            self.container_client = self.blob_client.get_container_client(
                container=self.container_name
            )
        except:
            logging.critical(
                "Authorization failed. Please connect this service to an valid blob storage"
            )
            traceback.print_exc()

    def create(
        self, model_result: ModelTrainingsResult, overwrite=True
    ) -> ModelLocation:
        """
        This method ensures the model will be persisted in storages.

        Possible outcomes:
        - OK:           Model pipeline has been succesfully been persisted.
        - BadRequest:   Model pipeline persistence has failed.
        """
        self.load_account()
        # Convert model to byte array
        model_pickle_bytes = dumps(model_result.model_pipeline)
        model_location = self._format_location(model_result)
        blob_client = self.container_client.get_blob_client(
            blob=model_location.ModelAddress
        )
        # Upload file to Azure blob storage.
        blob_client.upload_blob(model_pickle_bytes, overwrite=overwrite)
        logging.info(f"Upload of model {model_location} has been succesfull")
        return model_location

    def get_model(self, model_id: str) -> Pipeline | None:
        try:
            self.load_account()
            model_bytes = self.container_client.download_blob(model_id).readall()
            return loads(model_bytes)
        except:
            return None
    
    def delete_model(self, model_address: str):
        self.load_account()
        client = self.container_client.get_blob_client(blob=model_address)
        client.delete_blob()
        logging.info(f"Model has been deleted with the id: {model_address}")
        return DeleteDTO(message="Model has been deleted", has_deleted=True)

    def _format_location(self, model_result: ModelTrainingsResult) -> ModelLocation:
        """
        Formats location data to ModelLocation domain
        """
        return ModelLocation(
            WorkspaceGuid=model_result.model_config.ParentWorkspaceGuid,
            ModelName=model_result.model_config.ModelName,
            ModelAddress=self._format_address(model_result),
        )

    def _format_address(self, model_result: ModelTrainingsResult) -> str:
        """
        Returns an address
        """
        return FORMAT_MODEL_ID.format(
            workspace_id=model_result.model_config.ParentWorkspaceGuid,
            name=model_result.model_config.ModelName,
            model_type=model_result.model_config.ModelType.name,
        )

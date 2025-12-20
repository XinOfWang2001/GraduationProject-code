using Leap.ApplicationServices.DTO.DataConfig;
using Leap.ApplicationServices.DTO.DataResult;
using Leap.ApplicationServices.DTO.ModelingProcess;

namespace Leap.ApplicationServices.Interfaces.ExternalServiceAPI
{
    // Interface for communicating with the external Python service.
    // Responsibility:
    // - Is responsible as the central gateway to the Leap.FastAPI.
    // FastAPI Service is responsible for:
    // - Handling all the Data science workloads.
    // - This includes: Model training, Model inference and Model storage and management.
    public interface IPythonFacadeService
    {
        // Request preview data
        Task<PreviewDataDTO?> RequestPreviewData(DataRequestDTO request);
        // Trigger modeltraining, without model storage.
        Task<ModelResultDataDTO?> TriggerModelTraining(ModelRequestDTO request);

        // Trigger modeltraining and storage
        // Use model and request forecasting data from existing forecasting model
        Task<ModelStorageDTO?> StoreModel(ModelRequestDTO request);

        Task DeleteModel(string ModelAddress);
    }
}

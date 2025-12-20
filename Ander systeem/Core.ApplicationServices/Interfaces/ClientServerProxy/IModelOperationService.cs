using Leap.ApplicationServices.DTO.DataResult;
using Leap.ApplicationServices.DTO.ModelingProcess;

namespace Leap.ApplicationServices.Interfaces.ClientServerProxy
{
    // Responsibility
    // Trigger model training
    // Trigger model training and its storage/overriding it.
    // Use model to generate prediction data
    public interface IModelOperationService
    {
        // This method will trigger the model training. For now, both forecasting and outlier detection model training will be decided here.
        Task<ModelResultDataDTO?> TriggerModelTraining(ModelTrainingRequestDTO WorkspaceGuid);
        // Trigger model training, but with storing the model.
        Task<ModelStorageDTO?> TriggerModelStorage(ModelStorageCreationRequestDTO ModelStorageRequest);
        Task<ModelStorageDTO?> GetModelStorage(Guid WorkspaceGuid);

        Task<bool> DeleteModelFile(Guid WorkspaceGuid);
    }
}

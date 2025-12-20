using Leap.ApplicationServices.DTO.DataResult;
using Leap.ApplicationServices.DTO.ModelingProcess;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using LeapDataScienceTool.API;

namespace LeapDataScienceTool.Services.Proxies
{
    public class ModelOperationProxyService : IModelOperationService
    {
        private readonly IServerAPI serverAPI;

        public ModelOperationProxyService(IServerAPI serverAPI) => this.serverAPI = serverAPI;

        public async Task<ModelResultDataDTO?> TriggerModelTraining(ModelTrainingRequestDTO DTO)
        {
            return await serverAPI.Post<ModelResultDataDTO>("modeloperation/training-preview", DTO);
        }

        public async Task<ModelStorageDTO?> TriggerModelStorage(ModelStorageCreationRequestDTO ModelStorageRequest)
        {
            return await serverAPI.Post<ModelStorageDTO>("modeloperation/model-storage", ModelStorageRequest);
        }

        public async Task<ModelStorageDTO?> GetModelStorage(Guid WorkspaceGuid)
        {
            return await serverAPI.Get<ModelStorageDTO>($"modeloperation/model-storage/{WorkspaceGuid}");
        }

        public async Task<bool> DeleteModelFile(Guid WorkspaceGuid)
        {
            return await serverAPI.Delete($"modeloperation/remove/{WorkspaceGuid}");
        }
    }
}

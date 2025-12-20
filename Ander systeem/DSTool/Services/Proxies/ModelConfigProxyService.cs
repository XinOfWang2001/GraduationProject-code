using Leap.ApplicationServices.DTO.ModelDTO;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using LeapDataScienceTool.API;

namespace LeapDataScienceTool.Services.Proxies
{
    public class ModelConfigProxyService(IServerAPI serverAPI) : IModelService
    {
        private readonly IServerAPI serverAPI = serverAPI;

        public async Task<ModelConfigDTO?> GetModelConfig(Guid ConfigGuid)
        {
            var result = await serverAPI.Get<ModelConfigDTO>($"model/{ConfigGuid}");
            return result;
        }

        public async Task<ModelConfigDTO?> RegisterModelConfig(ModelConfigDTO modelConfigDto)
        {
            var result = await serverAPI.Post<ModelConfigDTO>("model", modelConfigDto);
            return result;
        }

        public async Task<ModelConfigDTO?> UpdateModelConfig(Guid ConfigGuid, ModelConfigDTO modelConfigDto)
        {
            var result = await serverAPI.Put<ModelConfigDTO>($"model/{ConfigGuid}", modelConfigDto);
            return result;
        }
    }
}

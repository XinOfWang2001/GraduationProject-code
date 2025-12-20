using Leap.ApplicationServices.DTO.ModelDTO;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using Leap.ApplicationServices.Interfaces.Creational;
using Leap.ApplicationServices.Interfaces.Repositories;
using Leap.Domain.Domain.ModelConfig;
using Leap.Domain.Domain.Workspaces;

namespace LeapDataScienceAPI.Services.Proxies
{
    public class ModelConfigService(
        IWorkspaceRepository workspaceRepository,
        IModelConfigRepository configRepository,
        IModelConfigBuilder configBuilder) : IModelService
    {
        private readonly IWorkspaceRepository workspaceRepository = workspaceRepository;
        private readonly IModelConfigRepository configRepository = configRepository;
        private readonly IModelConfigBuilder configBuilder = configBuilder;

        public async Task<ModelConfigDTO?> GetModelConfig(Guid ConfigGuid)
        {
            var result = await configRepository.GetOne(ConfigGuid);
            if (result == null)
            {
                return null;
            }
            var dto = configBuilder.BuildDTO(result);
            return dto;
        }

        public async Task<ModelConfigDTO?> RegisterModelConfig(ModelConfigDTO modelConfigDto)
        {
            Workspace? workspace = workspaceRepository.Get(modelConfigDto.ParentWorkspaceGuid) ?? throw new FileNotFoundException();
            ModelConfiguration configuration = configBuilder.BuildCompleteDomain(modelConfigDto, workspace);
            // Assuming config already exists. Throw error
            await configRepository.Create(configuration);

            return modelConfigDto;
        }

        public async Task<ModelConfigDTO?> UpdateModelConfig(Guid ConfigGuid, ModelConfigDTO modelConfigDto)
        {
            ModelConfiguration? existingModelConfig = await configRepository.GetOne(ConfigGuid) ?? throw new FileNotFoundException();
            ModelConfiguration? configuration = configBuilder.BuildCompleteDomain(modelConfigDto, existingModelConfig.ParentWorkspace);
            // Overwrite model config. Otherwise throw error.
            await configRepository.Update(ConfigGuid, configuration);
            return modelConfigDto;
        }
    }
}

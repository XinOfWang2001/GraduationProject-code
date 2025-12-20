using Leap.ApplicationServices.DTO.ModelDTO;

namespace Leap.ApplicationServices.Interfaces.ClientServerProxy
{
    public interface IModelService
    {
        Task<ModelConfigDTO?> RegisterModelConfig(ModelConfigDTO modelConfigDto);
        Task<ModelConfigDTO?> UpdateModelConfig(Guid ConfigGuid, ModelConfigDTO modelConfigDto);

        Task<ModelConfigDTO?> GetModelConfig(Guid ConfigGuid);

    }
}

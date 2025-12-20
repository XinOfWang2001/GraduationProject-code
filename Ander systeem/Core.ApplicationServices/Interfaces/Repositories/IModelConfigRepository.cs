using Leap.Domain.Domain.ModelConfig;

namespace Leap.ApplicationServices.Interfaces.Repositories
{
    public interface IModelConfigRepository
    {
        Task<ModelConfiguration?> GetOne(Guid guid);
        Task<ModelConfiguration> Create(ModelConfiguration configuration);
        Task<ModelConfiguration?> Update(Guid configGuid, ModelConfiguration configuration);
    }
}

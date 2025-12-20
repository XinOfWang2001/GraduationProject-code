using Leap.ApplicationServices.DTO.ModelDTO;
using Leap.Domain.Domain.ModelConfig;
using Leap.Domain.Domain.Workspaces;

namespace Leap.ApplicationServices.Interfaces.Creational
{
    public interface IModelConfigBuilder
    {
        protected ModelConfiguration BuildBase(ModelConfigDTO modelConfigDTO, Workspace workspace);
        ModelConfiguration BuildCompleteDomain(ModelConfigDTO dto, Workspace workspace);
        ModelConfigDTO BuildDTO(ModelConfiguration modelConfig);
    }
}

using Leap.ApplicationServices.DTO.Workspace;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using Leap.ApplicationServices.Interfaces.Repositories;
using Leap.Domain.Domain.Workspaces;
using LeapDataScienceAPI.Services.BuilderAndMappers.Mappers;

namespace LeapDataScienceAPI.Services.Proxies
{
    public class WorkspaceService(
        IWorkspaceRepository workspaceRepository,
        IModelService modelProxyService,
        IModelOperationService modelOperationService,
        ICalculationRepository calculationRepository
             ) : IWorkspaceService
    {
        private readonly IWorkspaceRepository workspaceRepository = workspaceRepository;
        private readonly IModelService modelProxyService = modelProxyService;
        private readonly IModelOperationService modelOperationService = modelOperationService;

        public async Task<bool> DeleteWorkspace(Guid workspaceId)
        {
            // Delete Model in FastAPI
            await modelOperationService.DeleteModelFile(workspaceId);
            // Delete entire workspace
            return workspaceRepository.Delete(workspaceId);
        }

        public async Task<IEnumerable<WorkspaceConfigDTO>> GetAllWorkspaces()
        {
            IEnumerable<Workspace> workspaces = await workspaceRepository.GetAll();
            // HIER een NULL-Exception
            return workspaces.Select(x =>
            new WorkspaceConfigDTO()
            {
                WorkshopId = x.WorkspaceId,
                WorkspaceName = x.Name,
                WorkspaceGuid = x.WorkspaceGuid
            });
        }

        public async Task<WorkspaceConfigDTO?> GetWorkspace(Guid workspaceId)
        {
            var result = workspaceRepository.Get(workspaceId);
            if (result == null)
            {
                return null;
            }

            WorkspaceConfigDTO dto = result.MapToDTO();
            if (result.ModelConfig != null)
            {
                var modelConfig = await modelProxyService.GetModelConfig(result.ModelConfig.ModelConfigGuid);
                dto.ModelConfigDTO = modelConfig;
            }
            // Get optional calculation steps
            var steps = calculationRepository.Get(workspaceId);
            dto.CalculationStepsDTO = steps.Select(step => step.MapToDTO());
            return dto;
        }

        public Task<WorkspaceConfigDTO?> RegisterWorkspace(WorkspaceConfigDTO dto)
        {

            Workspace workspace = dto.MapToDomain();
            workspaceRepository.Create(workspace);
            return Task.FromResult(dto);
        }
    }
}

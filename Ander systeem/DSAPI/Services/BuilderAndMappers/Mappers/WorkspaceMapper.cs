using Leap.ApplicationServices.DTO.Workspace;
using Leap.Domain.Domain.Workspaces;

namespace LeapDataScienceAPI.Services.BuilderAndMappers.Mappers
{
    public static class WorkspaceMapper
    {
        public static WorkspaceConfigDTO MapToDTO(this Workspace workspace)
        {
            return new WorkspaceConfigDTO()
            {
                WorkshopId = workspace.WorkspaceId,
                WorkspaceGuid = workspace.WorkspaceGuid,
                WorkspaceName = workspace.Name,
                DataSourceConfig = workspace.DataExtraction?.MapToDTO()
            };
        }

        public static Workspace MapToDomain(this WorkspaceConfigDTO workspace)
        {
            return new Workspace()
            {
                Name = workspace.WorkspaceName,
                WorkspaceGuid = workspace.WorkspaceGuid,

            };
        }
    }
}

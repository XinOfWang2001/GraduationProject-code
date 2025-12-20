using Leap.ApplicationServices.DTO.Workspace;

namespace Leap.ApplicationServices.Interfaces.ClientServerProxy
{
    public interface IWorkspaceService
    {
        Task<WorkspaceConfigDTO?> RegisterWorkspace(WorkspaceConfigDTO workspace);
        Task<WorkspaceConfigDTO?> GetWorkspace(Guid workspaceId);
        Task<IEnumerable<WorkspaceConfigDTO>> GetAllWorkspaces();
        Task<bool> DeleteWorkspace(Guid workspaceId);
    }
}

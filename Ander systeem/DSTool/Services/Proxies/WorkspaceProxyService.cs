using Leap.ApplicationServices.DTO.Workspace;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using LeapDataScienceTool.API;

namespace LeapDataScienceTool.Services.Proxies
{
    public class WorkspaceProxyService : IWorkspaceService
    {
        private readonly IServerAPI serverAPI;

        public WorkspaceProxyService(IServerAPI serverAPI)
        {
            this.serverAPI = serverAPI;
        }

        public Task<bool> DeleteWorkspace(Guid workspaceId)
        {
            // Send request to
            return serverAPI.Delete($"workspace/{workspaceId}");
        }

        public async Task<IEnumerable<WorkspaceConfigDTO>> GetAllWorkspaces()
        {
            return await serverAPI.GetAll<WorkspaceConfigDTO>("workspace");
        }

        public async Task<WorkspaceConfigDTO?> GetWorkspace(Guid workspaceId)
        {
            return await serverAPI.Get<WorkspaceConfigDTO>($"workspace/{workspaceId}");
        }

        public async Task<WorkspaceConfigDTO?> RegisterWorkspace(WorkspaceConfigDTO workspace)
        {
            var result = await serverAPI.Post<WorkspaceConfigDTO>("workspace", workspace);
            return result;
        }
    }
}

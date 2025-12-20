using Leap.ApplicationServices.DTO.DataResult;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using LeapDataScienceTool.API;

namespace LeapDataScienceTool.Services
{
    public class PreviewDataProxyService(IServerAPI serverAPI) : IPreviewDataService
    {
        private readonly IServerAPI serverAPI = serverAPI;

        public async Task<PreviewDataDTO?> GetPreviewData(Guid workspaceGuid, bool ProvideData = false)
        {
            var response = await serverAPI.Get<PreviewDataDTO>($"datasource/{workspaceGuid}/preview-data");
            return response;
        }
    }
}

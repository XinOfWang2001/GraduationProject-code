using Leap.ApplicationServices.DTO.External_Services;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using LeapDataScienceTool.API;

namespace LeapDataScienceTool.Services.Proxies
{
    public class SwecoDataSourceService(IServerAPI serverAPI) : IMonitorDataService
    {
        private readonly IServerAPI ServerAPI = serverAPI;

        public async Task<MonitorInfoDTO?> GetMonitorInfoAsync(MonitorInfoRequest request)
        {
            MonitorInfoDTO? monitorInfoDTO = await ServerAPI.Get<MonitorInfoDTO>($"datasource/project/{request.ProjectId}");
            return monitorInfoDTO;
        }
    }
}

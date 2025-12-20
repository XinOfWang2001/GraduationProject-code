using Leap.ApplicationServices.DTO.External_Services;

namespace Leap.ApplicationServices.Interfaces.ClientServerProxy
{
    public interface IMonitorDataService
    {
        Task<MonitorInfoDTO?> GetMonitorInfoAsync(MonitorInfoRequest request);

    }
}

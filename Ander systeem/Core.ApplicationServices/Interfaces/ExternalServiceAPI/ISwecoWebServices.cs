using Leap.ApplicationServices.DTO.External_Services;

namespace Leap.ApplicationServices.Interfaces.ExternalServiceAPI
{
    public interface ISwecoWebServices<T> where T : ISwecoWebServices<T>
    {
        Task<MonitorInfoDTO?> GetInfo(string project, string token);
    }
}

using Leap.ApplicationServices.DTO.External_Services;
using Leap.ApplicationServices.Interfaces.ExternalServiceAPI;
using System.Net.Http.Json;

namespace Leap.ApplicationServices.AppGeneralServices.ExternalServices
{
    public class IWAWebService : ISwecoWebServices<IWAWebService>
    {
        private readonly IHttpClientFactory httpClientFactory;
        private HttpClient httpClient { get; set; }

        public IWAWebService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
            httpClient = this.httpClientFactory.CreateClient("IWA_Server");
        }

        public async Task<MonitorInfoDTO?> GetInfo(string project, string token)
        {
            string completeProjectUrl = GetProjectInfoUrl(project, token);
            var response = await httpClient.GetAsync(completeProjectUrl);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MonitorInfoDTO>();
            }
            return null;
        }

        private string GetProjectInfoUrl(string project, string token)
        {
            return $"{project}/data/monitor-info?token={token}";
        }
    }
}

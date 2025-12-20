using Leap.ApplicationServices.DTO.DataConfig;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using LeapDataScienceTool.API;

namespace LeapDataScienceTool.Services.Proxies
{
    public class DataExtractProxyService
        (IServerAPI serverAPI) : IDataExtractService
    {
        private readonly IServerAPI serverAPI = serverAPI;

        public async Task<DataExtractConfigDTO?> RegisterDataExtractProcess(DataExtractConfigDTO config)
        {
            return await serverAPI.Post<DataExtractConfigDTO>("dataextract", config);
        }

        public async Task<DataExtractConfigDTO?> UpdateDataExtractProcess(Guid procesId, DataExtractConfigDTO config)
        {
            return await serverAPI.Put<DataExtractConfigDTO>($"dataextract/{procesId}", config);
        }
    }
}

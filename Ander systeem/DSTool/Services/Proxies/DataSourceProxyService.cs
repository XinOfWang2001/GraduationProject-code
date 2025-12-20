using Leap.ApplicationServices.DTO.DataProcessDTO;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using LeapDataScienceTool.API;

namespace LeapDataScienceTool.Services.Proxies
{
    public class DataSourceProxyService(IServerAPI serverAPI) : IDataSourceService
    {
        private readonly IServerAPI serverAPI = serverAPI;

        public async Task<IEnumerable<DataSourceDTO>> GetData()
        {
            var dataSources = await serverAPI.GetAll<DataSourceDTO>("datasource");
            return dataSources;
        }

        public async Task<DataSourceDTO?> GetOne(int id)
        {
            var dataSource = await serverAPI.Get<DataSourceDTO>($"datasource/{id}");
            return dataSource;
        }
    }
}

using Leap.ApplicationServices.DTO.DataProcessDTO;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using Leap.ApplicationServices.Interfaces.Repositories;
using Leap.Domain.Domain.DataSource;
using LeapDataScienceAPI.Services.BuilderAndMappers.Mappers;

namespace LeapDataScienceAPI.Services.Proxies
{
    public class DataSourceService(IDataSourceRepo<SwecoDataSource> dataSourceRepo) : IDataSourceService
    {
        public Task<IEnumerable<DataSourceDTO>> GetData()
        {
            IEnumerable<SwecoDataSource> dataSources = dataSourceRepo.GetAll();
            return Task.FromResult(dataSources.Select(ds => ds.MapToDTO()));
        }

        public Task<DataSourceDTO?> GetOne(int id)
        {
            // Gets all the datasources.
            SwecoDataSource? dataSources = dataSourceRepo.Get(id);
            if (dataSources == null)
            {
                return Task.FromResult(new DataSourceDTO() { DataSourceId = 1, StatusCode = 404, Message = "Databron niet gevonden" });
            }
            DataSourceDTO dataSourcesDTO = dataSources.MapToDTO();
            // Verstuur projecten
            return Task.FromResult(dataSourcesDTO);
        }
    }
}

using Leap.ApplicationServices.DTO.DataProcessDTO;

namespace Leap.ApplicationServices.Interfaces.ClientServerProxy
{
    public interface IDataSourceService
    {
        Task<IEnumerable<DataSourceDTO>> GetData();
        Task<DataSourceDTO?> GetOne(int id);
    }
}

using Leap.Domain.Domain.DataSource;

namespace Leap.ApplicationServices.Interfaces.Repositories
{
    public interface IDataSourceRepo<T> where T : SwecoDataSource
    {
        T? Get(int id);
        IEnumerable<T> GetAll();
    }
}

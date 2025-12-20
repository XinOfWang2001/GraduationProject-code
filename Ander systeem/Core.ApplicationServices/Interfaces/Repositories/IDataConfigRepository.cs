using Leap.Domain.Domain.DataConfig;

namespace Leap.ApplicationServices.Interfaces.Repositories
{
    public interface IDataConfigRepository
    {
        void Update(DataSourceConfig sourceConfig);
    }
}

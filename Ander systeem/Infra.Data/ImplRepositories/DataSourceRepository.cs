using Infra.Data.DatabaseContext;
using Leap.ApplicationServices.Interfaces.Repositories;
using Leap.Domain.Domain.DataSource;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.ImplRepositories
{
    public class DataSourceRepository : IDataSourceRepo<SwecoDataSource>
    {
        private readonly LeapDSDBContext leapDSDBContext;

        public DataSourceRepository(LeapDSDBContext leapDSDBContext)
        {
            this.leapDSDBContext = leapDSDBContext;
        }
        public SwecoDataSource? Get(int id)
        {
            var result = leapDSDBContext.
                    IWADataSources
                    .Where(data => data.DataSourceId == id)
                    .Include(ds => ds.Projects)
                    .FirstOrDefault();
            return result;
        }

        public IEnumerable<SwecoDataSource> GetAll()
        {
            return leapDSDBContext.SwecoDataSources
                .Include(ds => ds.Projects)
                .AsEnumerable();
        }
    }
}

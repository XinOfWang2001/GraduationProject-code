using Infra.Data.DatabaseContext;
using Leap.ApplicationServices.Interfaces.Repositories;
using Leap.Domain.Domain.Workspaces;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.ImplRepositories
{
    public class WorkspaceRepository : IWorkspaceRepository
    {
        private readonly LeapDSDBContext dbContext;

        public WorkspaceRepository(LeapDSDBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public Workspace? Create(Workspace workspace)
        {
            dbContext.Add(workspace);
            dbContext.SaveChanges();
            return workspace;
        }

        public bool Delete(Guid workspaceGuid)
        {
            var result = dbContext.Workspace.FirstOrDefault(ws => ws.WorkspaceGuid == workspaceGuid);
            if (result != null)
            {
                dbContext.Remove(result);
                dbContext.SaveChanges();
            }
            return true;
        }

        public Workspace? Get(Guid guid)
        {
            var result = dbContext.Workspace
                .AsSplitQuery()
                .Include(ds => ds.DataExtraction)
                    .ThenInclude(x => x.DataSourceConfig)
                        .ThenInclude(ds => ds.AssignedProject)
                .Include(ds => ds.DataExtraction)
                    .ThenInclude(ds => ds.DataSourceConfig)
                        .ThenInclude(ds => ds.Sensors)
                .Include(ds => ds.DataExtraction)
                    .ThenInclude(ds => ds.DataSourceConfig)
                        .ThenInclude(ds => ds.ValueTypes)
                .Include(mc => mc.ModelConfig)
                    .ThenInclude(mc => mc.ModelParameters)
                .Include(mc => mc.ModelConfig)
                    .ThenInclude(mc => mc.FeatureColumns)
                .Include(mc => mc.ModelConfig)
                    .ThenInclude(mc => mc.TargetColumns)
                .FirstOrDefault(ws => ws.WorkspaceGuid == guid);

            return result;
        }

        public async Task<IEnumerable<Workspace>> GetAll()
        {
            return await dbContext.Workspace.Include(ds => ds.DataExtraction).ToListAsync();
        }

        public Workspace? Update(Workspace workspace)
        {
            try
            {
                dbContext.Workspace.Update(workspace);
                dbContext.SaveChanges();
                return workspace;
            }
            catch
            {
                return null;
            }
        }
    }
}

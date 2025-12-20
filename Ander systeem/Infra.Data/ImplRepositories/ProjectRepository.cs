using Infra.Data.DatabaseContext;
using Leap.ApplicationServices.Interfaces.Repositories;
using Leap.Domain.Domain.DataSource;

namespace Infra.Data.ImplRepositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly LeapDSDBContext leapDSDBContext;

        public ProjectRepository(LeapDSDBContext leapDSDBContext)
        {
            this.leapDSDBContext = leapDSDBContext;
        }

        public Project? Get(Guid projectGuidId)
        {
            return leapDSDBContext.Project
                .Where(proj => proj.ProjectGuid == projectGuidId)
                .FirstOrDefault();
        }

        public Project? Get(int projectId)
        {
            return leapDSDBContext.Project.Find(projectId);
        }
    }
}

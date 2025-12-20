using Leap.Domain.Domain.DataSource;

namespace Leap.ApplicationServices.Interfaces.Repositories
{
    public interface IProjectRepository
    {
        Project? Get(Guid projectGuidId);
        Project? Get(int projectId);
    }
}

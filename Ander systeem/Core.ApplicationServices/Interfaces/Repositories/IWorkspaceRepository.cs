using Leap.Domain.Domain.Workspaces;

namespace Leap.ApplicationServices.Interfaces.Repositories
{
    public interface IWorkspaceRepository
    {
        Workspace? Create(Workspace workspace);
        Workspace? Update(Workspace workspace);
        Workspace? Get(Guid guid);
        Task<IEnumerable<Workspace>> GetAll();

        bool Delete(Guid workspaceGuid);
    }
}

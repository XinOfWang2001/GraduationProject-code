using Leap.Domain.Domain.DataConfig;

namespace Leap.ApplicationServices.Interfaces.Repositories
{
    public interface IDataExtractRepository
    {
        DataExtracter? GetByWorkspace(Guid workspaceGuid);
        DataExtracter? Get(Guid procesId);
        IEnumerable<DataExtracter> Get();
        Task<DataExtracter?> Create(DataExtracter dataExtracter);
        Task<DataExtracter?> Update(Guid procesId, DataExtracter dataExtracter);
    }
}

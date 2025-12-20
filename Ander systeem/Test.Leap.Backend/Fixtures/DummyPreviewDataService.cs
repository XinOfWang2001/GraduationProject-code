using Leap.ApplicationServices.DTO.DataResult;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;

namespace Test.Leap.Backend.Fixtures
{
    public class DummyPreviewDataService : IPreviewDataService
    {
        private static PreviewDataDTO InMemoryColumnData(Guid WorkspaceGuid)
        {
            return new PreviewDataDTO
            {
                DataColumns = [new() { ColumnName = "KT", DataType = "f64" }, new() { ColumnName = "KT", DataType = "f64" }],
                DataCount = 5,
                StatusCode = 200,
                Message = string.Empty,
                DataSet = new DataSeries() { ColumnNames = ["KT1", "KT"], Timestamps = [], Values = [] },
            };
        }
        public Task<PreviewDataDTO?> GetPreviewData(Guid workspaceGuid, bool ProvideData = false)
        {
            return Task.FromResult(InMemoryColumnData(workspaceGuid));
        }
    }
}

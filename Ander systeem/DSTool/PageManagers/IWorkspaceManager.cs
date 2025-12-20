using Leap.ApplicationServices.DTO;
using Leap.ApplicationServices.DTO.DataConfig;
using Leap.ApplicationServices.DTO.ModelDTO;
using Leap.ApplicationServices.DTO.Workspace;

namespace LeapDataScienceTool.PageManagers
{
    public interface IWorkspaceManager
    {

        event EventHandler<DataSourceEventArgs> DataSourceChanged;
        // Future event handlers added

        Task LoadAllAssets(Guid WorkspaceGuid);
        Task UpdateDataSourceConfig(DataExtractConfigDTO config);
        void UpdateModelConfig(ModelConfigDTO modelConfig);
        Task UpdateDataColumns();
        ModelConfigDTO? GetModelConfig();
        DataExtractConfigDTO? GetDataExtractConfigDTO();
        WorkspaceConfigDTO GetWorkspaceConfigDTO();
        IEnumerable<DataColumnDTO> GetColumns();
    }

    public class DataSourceEventArgs : EventArgs
    {
        public string Name { get; set; }
        public int FeatureChanges { get; set; }
        public int TargetChanges { get; set; }
    }
}

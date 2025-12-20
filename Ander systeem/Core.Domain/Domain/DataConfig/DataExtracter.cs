using Leap.Domain.Domain.Workspaces;

namespace Leap.Domain.Domain.DataConfig
{
    // This is a specific abstraction of dataprocess, because the extracter is part of the Modeling process steps
    public class DataExtracter
    {
        public int DataProcessId { get; set; }
        public Guid ProcessId { get; set; } = Guid.NewGuid();

        // FK relation to workspace class
        public int ParentWorkspaceId { get; set; }
        public Workspace? ParentWorkspace { get; set; }

        public required DataSourceConfig DataSourceConfig { get; set; }

        public void UpdateExtracter(DataExtracter inputExtracter)
        {
            DataSourceConfig.UpdateConfig(inputExtracter.DataSourceConfig);
        }
    }
}

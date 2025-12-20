using Leap.Domain.Domain.DataConfig;

namespace Leap.Domain.Domain.DataSource
{
    // Only being used by SwecoDataSource
    public class Project
    {
        public int Id { get; set; }
        public Guid ProjectGuid { get; set; }
        public required string Name { get; set; }
        public required string HumanReadableName { get; set; }
        public string ProjectToken { get; set; } = string.Empty;

        // FK One to many relation
        public SwecoDataSource SwecoDataSource { get; set; }
        public List<DataSourceConfig> DataSourceConfigs { get; set; } = [];
        public List<SensorObject> Observations { get; set; } = [];
        public List<ValueTypes> ValueTypes { get; set; } = [];
        public int SwecoDataSourceDataSourceId { get; set; }
    }
}

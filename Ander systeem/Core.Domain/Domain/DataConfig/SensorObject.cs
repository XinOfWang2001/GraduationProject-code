using Leap.Domain.Domain.DataSource;

namespace Leap.Domain.Domain.DataConfig
{
    // This will only store the chosen valuetype
    public class SensorObject
    {
        // PK, SensorId and ValueTypeId's will be made unique
        public int SensorId { get; set; }
        public string SensorName { get; set; } = string.Empty;
        // FK_Relation to Project
        public int ProjectId { get; set; }
        public Guid SensorGuid { get; set; } = Guid.NewGuid();
        public List<DataSourceConfig> Configs { get; set; } = [];
        public Project? Project { get; set; }
    }
}

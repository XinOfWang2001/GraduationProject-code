using Leap.Domain.Domain.DataSource;

namespace Leap.Domain.Domain.DataConfig
{
    public class ValueTypes
    {
        public int ValueTypeId { get; set; }
        // FK_Relation to Project
        public int ProjectId { get; set; }
        public string ValueTypeName { get; set; } = string.Empty;

        // AK
        public Guid VTGuid { get; set; } = Guid.NewGuid();
        // FK_Relation to config class
        public List<DataSourceConfig> Configs { get; set; } = [];
        public Project? Project { get; set; }
    }
}

namespace Core.Domain.Domain.ModelConfig
{
    public class DataColumns
    {
        public int DataColumnsId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        // Foreign key relation to ModelConfiguration
        public ModelConfiguration ParentConfiguration { get; set; }
        public int ParentConfigurationId { get; set; }
    }

    public class FeatureColumns : DataColumns
    {

    }
    public class TargetColumns : DataColumns
    {

    }
}

using Core.Domain.Domain.ModelConfig.Enums;
using Core.Domain.Domain.ModelConfig.ModelParams;
using Core.Domain.Domain.Workspaces;


namespace Leap.Domain.Domain.ModelConfig
{
    public class ModelConfiguration
    {
        public int ModelConfigId { get; set; }
        public Guid ModelConfigGuid { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public float DataSplitRatio { get; set; } = 0.7f; // Standard 70% training, 20% testing
        public DateTime DateForecasting { get; set; } = DateTime.Now.AddDays(30); // Standard 30 days into the future.
        public DateTimeLevel DateTimeLevel { get; set; } = DateTimeLevel.STANDARD;
        public ModelType ModelType { get; set; }
        public ModelAlgorithm ModelAlgorithm { get; set; }
        public required ModelParameters ModelParameters { get; set; }
        public ICollection<FeatureColumns> FeatureColumns { get; set; } = [];
        public ICollection<TargetColumns> TargetColumns { get; set; } = [];

        // Has a foreign key relation of Workspace table
        public required Workspace ParentWorkspace { get; set; }
        public int ParentWorkspaceId { get; set; }

        public long GetDateTimeUnix()
        {
            return new DateTimeOffset(DateForecasting).ToUnixTimeMilliseconds();
        }

        public bool ValidFeatureColumnSettings()
        {
            if (ModelType.Equals(ModelType.OUTLIER_DETECTION))
            {
                return FeatureColumns.Count > 0;
            }
            return true;
        }

        public bool ValidTargetColumnSettings()
        {
            if (ModelType.Equals(ModelType.FORECASTING))
            {
                return TargetColumns.Count > 0;
            }
            return true;
        }

        public void UpdateEntity(ModelConfiguration configuration)
        {
            Name = configuration.Name;
            DataSplitRatio = configuration.DataSplitRatio;
            DateForecasting = configuration.DateForecasting;
            DateTimeLevel = configuration.DateTimeLevel;
            ModelType = configuration.ModelType;
            ModelAlgorithm = configuration.ModelAlgorithm;
            ModelParameters = configuration.ModelParameters;
            FeatureColumns = configuration.FeatureColumns;
            TargetColumns = configuration.TargetColumns;
        }
    }
}

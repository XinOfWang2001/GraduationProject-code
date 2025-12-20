using Leap.Domain.Domain.ModelConfig.Enums;

namespace Leap.ApplicationServices.DTO.ModelDTO
{
    public class ModelConfigDTO
    {
        public Guid ModelConfigGuid { get; set; } = Guid.NewGuid();
        public required Guid ParentWorkspaceGuid { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public float DataSplitRatio { get; set; } = 0.5f;
        public DateTime? ForecastingDate { get; set; } = DateTime.Now.AddDays(7);
        public DateTimeLevel DateTimeLevel { get; set; } = DateTimeLevel.STANDARD;
        public ModelType ModelType { get; set; }
        public ModelAlgorithm ModelAlgorithm { get; set; }
        public IEnumerable<DataColumnDTO> Features { get; set; } = [];
        public IEnumerable<DataColumnDTO> Targets { get; set; } = [];
        public AlgorithmDTO AlgorithmParameterDTO { get; set; } = new LinearRegressionDTO();
    }
}

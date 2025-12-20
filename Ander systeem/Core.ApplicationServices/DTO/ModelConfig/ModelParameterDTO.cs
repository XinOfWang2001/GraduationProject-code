using System.Text.Json.Serialization;

namespace Leap.ApplicationServices.DTO.ModelDTO
{
    [JsonDerivedType(typeof(LinearRegressionDTO), typeDiscriminator: "LinearRegressionDTO")]
    [JsonDerivedType(typeof(SVMDTO), typeDiscriminator: "SVMDTO")]

    public abstract class AlgorithmDTO
    {
        public int Id { get; set; }
        public string TypeOfAlgorithm { get; set; } = string.Empty;
    }

    public class LinearRegressionDTO : AlgorithmDTO
    {
        public int NJobs { get; set; } = 3;
    }

    public class SVMDTO : AlgorithmDTO
    {
        public string Kernel { get; set; } = "sigmoid";
    }
}

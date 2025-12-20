using System.Text.Json.Serialization;

namespace Leap.ApplicationServices.DTO.Calculations
{
    [JsonDerivedType(typeof(KPIDTO), typeDiscriminator: "KPIDTO")]
    public class CalculationDTO
    {
        public int CalculationId { get; set; }
        public required string OutputColumn { get; set; }
        public required IEnumerable<string> InputColumns { get; set; } = new List<string>();
    }

    public class KPIDTO : CalculationDTO
    {
        public required string CalculationString { get; set; } // Maps to CalculationString
        public required IEnumerable<string> OperationsList { get; set; } // Maps to ConcatCalculationString
    }
}

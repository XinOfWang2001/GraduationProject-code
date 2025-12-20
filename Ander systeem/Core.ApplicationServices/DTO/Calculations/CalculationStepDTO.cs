using Leap.Domain.Domain.Calculations;

namespace Leap.ApplicationServices.DTO.Calculations
{
    public class CalculationStepDTO
    {
        public int StepId { get; set; }
        public required int Order { get; set; }
        public CalculationType CalculationType { get; set; }
        public required IEnumerable<CalculationDTO> Calculations { get; set; } = [];
    }
}

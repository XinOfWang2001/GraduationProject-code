namespace Leap.ApplicationServices.DTO.Calculations
{
    public class CalculationRequestDTO : IDTO
    {
        public required IEnumerable<CalculationStepDTO> Steps { get; set; }
    }
}

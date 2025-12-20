namespace Leap.ApplicationServices.DTO.Calculations
{
    public class CalculationWriteDTO : IDTO
    {
        public Guid WorkspaceGuid { get; set; }

        public required IEnumerable<CalculationStepDTO> Steps { get; set; }
    }
}

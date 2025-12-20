namespace Leap.ApplicationServices.DTO.DataResult
{
    public class MetricsDTO
    {
        public required string Metric { get; set; }
        public required string Column { get; set; }
        public required float Value { get; set; }
    }
}

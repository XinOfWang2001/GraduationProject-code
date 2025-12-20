namespace Leap.ApplicationServices.DTO.DataConfig
{
    public class DataRequestDTO
    {
        // Data request class used to be send to the Python service.
        public string WorkspaceId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string DataSource { get; set; } = string.Empty;
        public long StartDateUnix { get; set; }
        public long EndDateUnix { get; set; }
        public string Project { get; set; } = string.Empty;
        public int Points { get; set; } = 5;
        public int? Timelevel { get; set; }
        public float? TimelevelRange { get; set; }
        public IEnumerable<int> ObservationIds { get; set; } = [];
        public IEnumerable<int> ValueTypeIds { get; set; } = [];
        public bool ProvideData { get; set; } = false;
    }
}

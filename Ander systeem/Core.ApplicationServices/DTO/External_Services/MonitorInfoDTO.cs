using Leap.ApplicationServices.DTO.DataConfig;

namespace Leap.ApplicationServices.DTO.External_Services
{
    // These are the DTO's used to call the IWA API's.

    // Response body of the monitor-info endpoint
    public class MonitorInfoDTO : IDTO
    {
        public MonitorObservationData[] Observations { get; set; } = [];
        public MonitorInfoValueType[] Valuetypes { get; set; } = [];
        public TimeLevelDTO[] TimeLevels { get; set; } = [];
    }
    // Request body for retrieving.
    public class MonitorInfoRequest
    {
        public int DataSourceId { get; set; }
        public string DataSourceType { get; set; }
        public int ProjectId { get; set; }
        public string Token { get; set; }
    }

    public class MonitorObservationData : SensorDTO
    {
        public int[] ValueTypeIds { get; init; }
    }

    public class MonitorInfoValueType : ValueTypeDTO
    {
        public string UnitAbbr { get; set; }
        public string Quantity { get; set; }
    }

    public class TimeLevelDTO
    {
        public int TimelevelId { get; set; } = -1;
        public string? TimelevelName { get; set; }
        // Timelevelrange is noted in Nanoseconds
        public float? TimelevelRange { get; set; }

        public override string ToString()
        {
            return TimelevelName;
        }
    }
}

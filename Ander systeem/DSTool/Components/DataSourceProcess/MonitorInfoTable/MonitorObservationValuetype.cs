using Leap.ApplicationServices.DTO.External_Services;

namespace LeapDataScienceTool.Components.DataSourceProcess.MonitorInfoTable
{
    public class MonitorObservationValuetype
    {
        public int Id { get; init; }
        public string Name { get; set; }
        public List<MonitorInfoValueType> ValueTypes { get; set; }
    }
}

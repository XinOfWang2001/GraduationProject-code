using Leap.ApplicationServices.DTO.DataConfig;
using LeapDataScienceTool.Common.Énums;

namespace LeapDataScienceTool.Components.DataSourceProcess
{
    public class DataExtractProcess
    {
        public DataExtractConfigDTO DataProcess { get; set; }
        public string Name { get; set; } = "Databron";
        public DataProcesState Status { get; set; } = DataProcesState.CONCEPT;
    }
}

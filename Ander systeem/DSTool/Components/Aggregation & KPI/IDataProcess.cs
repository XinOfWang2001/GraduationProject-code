using Leap.ApplicationServices.DTO.Calculations;

namespace LeapDataScienceTool.Components.Aggregation___KPI
{
    public abstract class IDataProcess
    {
        public string DataProcessType { get; set; } = string.Empty;
        public string Zone { get; set; }
        public int Order { get; set; } = -1;
        public bool IsPicked { get; set; } = false;

        public CalculationStepDTO CalculationStep { get; set; }

        public IDataProcess(string dataProcessType, string zone)
        {
            DataProcessType = dataProcessType;
            Zone = zone;
        }

        public abstract IDataProcess DeepCopy();
    }
}

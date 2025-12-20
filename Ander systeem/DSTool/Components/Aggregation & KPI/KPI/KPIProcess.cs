using Leap.ApplicationServices.DTO.Calculations;

namespace LeapDataScienceTool.Components.Aggregation___KPI
{
    // First changes
    public class KPIProcess : IDataProcess
    {
        //Should maintain

        public KPIProcess(string zone) : base(nameof(KPIProcess), zone)
        {

        }

        public KPIProcess(string dataProcessType, string zone, CalculationStepDTO step) : base(dataProcessType, zone)
        {
            CalculationStep = new CalculationStepDTO() { Order = step.Order, Calculations = [] };
        }

        public override IDataProcess DeepCopy()
        {
            return new KPIProcess(this.DataProcessType, this.Zone, this.CalculationStep);
        }
    }
}

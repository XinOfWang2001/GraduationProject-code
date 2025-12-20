namespace LeapDataScienceTool.Components.Aggregation___KPI
{
    public class AggregationProcess : IDataProcess
    {
        public AggregationProcess(string Zone) : base(nameof(AggregationProcess), Zone) { }
        public AggregationProcess(string dataProcessType, string zone) : base(dataProcessType, zone)
        {
        }

        public override IDataProcess DeepCopy()
        {
            return new AggregationProcess(this.DataProcessType, this.Zone);
        }
    }
}

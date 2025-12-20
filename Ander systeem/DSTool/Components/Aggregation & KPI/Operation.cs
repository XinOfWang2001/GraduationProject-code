namespace LeapDataScienceTool.Components.Aggregation___KPI
{
    public class Operation
    {
        public string Column { get; set; }
        public string Operator { get; set; }
        public string Id { get; set; }
        public Operation() { }

        public Operation(string Column, string Operator, string Id)
        {
            this.Column = Column;
            this.Operator = Operator;
            this.Id = Id;
        }

        public Operation DeepCopy()
        {
            return new Operation(Column, Operator, Id);
        }
    }
}

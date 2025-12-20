namespace Leap.Domain.Domain.Calculations
{
    public abstract class Calculation
    {
        public int CalculationId { get; set; }
        // Only database relevant.
        public string CalculationType { get; set; }
        public required string OutputColumn { get; set; }
        // Must be concated string: [Column]|[Column]|[Column]
        public required string InputColumns { get; set; }

        public required CalculationStep CalculationStep { get; set; }
        public int CalculationStepId { get; set; }

        protected readonly string DELIMITER = ",";

        public void Update(Calculation calculation)
        {
            OutputColumn = calculation.OutputColumn;
            InputColumns = calculation.InputColumns;
        }
        public IEnumerable<string> GetInputColumns()
        {
            return InputColumns.Split(DELIMITER).Where(t => !string.IsNullOrEmpty(t));
        }
    }
}


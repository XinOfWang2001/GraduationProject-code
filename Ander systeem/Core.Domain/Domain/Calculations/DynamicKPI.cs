namespace Leap.Domain.Domain.Calculations
{
    public class DynamicKPI : Calculation
    {
        // Example Column + Column 
        public required string CalculationString { get; set; }
        // Column,Operator,Column
        public required string ConcatCalculationString { get; set; }

        public void Update(DynamicKPI kPI)
        {
            CalculationString = kPI.CalculationString;
            ConcatCalculationString = kPI.ConcatCalculationString;
            base.Update(kPI);
        }

        public IEnumerable<string> GetCalculationArray()
        {
            return ConcatCalculationString.Split(DELIMITER).Where(t => !string.IsNullOrEmpty(t));
        }
    }
}

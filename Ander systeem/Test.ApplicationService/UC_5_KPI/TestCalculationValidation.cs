using Leap.ApplicationServices.AppGeneralServices.CalculationValidators;
using Leap.Domain.Domain.Calculations;
using Leap.Domain.Domain.Workspaces;

namespace Test.ApplicationService.UC_5_KPI
{
    public class TestCalculationValidation
    {
        // NA-XX Test Calculation validator if amount calculations == 0 fails
        [Fact]
        public void TestIfNoCalculationsFail()
        {
            CalculationInputValidator calculationInputValidator = new CalculationInputValidator();

            CalculationStep invalid = new() { CalculationType = CalculationType.KPI, Order = 1, Workspace = new Workspace() };
            IEnumerable<CalculationStep> list = [invalid];
            bool result = calculationInputValidator.Parse(list);
            string actualError = calculationInputValidator.ReturnError();
            Assert.False(result);
            Assert.Equal("Amount of calculations should be at least 1", actualError);
        }
        // NA-XX Test Calculation validator if amount calculations > 0 passes
        [Fact]
        public void TestIfCalculationStepsContainAtLeastOneCalculation()
        {
            CalculationInputValidator calculationInputValidator = new();

            CalculationStep valid = new() { CalculationType = CalculationType.KPI, Order = 1, Workspace = new Workspace() };

            DynamicKPI dynamicKPI = new() { CalculationStep = valid, CalculationString = "SA-W-1_d-voeg_mm + 2", ConcatCalculationString = "SA-W-1_d-voeg_mm,+,2", OutputColumn = "result1", InputColumns = "SA-W-1_d-voeg_mm" };
            valid.Calculations.Add(dynamicKPI);

            IEnumerable<CalculationStep> list = new List<CalculationStep>() { valid };
            bool result = calculationInputValidator.Parse(list);

            Assert.True(result);
        }
    }
}

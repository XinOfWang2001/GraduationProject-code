using Leap.ApplicationServices.AppGeneralServices.CalculationValidators;
using Leap.ApplicationServices.Interfaces.CalculationValidation;
using Leap.Domain.Domain.Calculations;
using Leap.Domain.Domain.Workspaces;

namespace Test.ApplicationService.UC_5_KPI
{
    public class TestComposite
    {
        // NA-XX Test valid input and calculation
        [Fact]
        public void TestCompositeWithValidInput()
        {
            CalculationFactory calculationFactory = new CalculationFactory();
            ICalculationComponent component = calculationFactory.CreateCalculationValidators();
            KPIParser kPIParser = new KPIParser();
            CalculationStep step = new() { CalculationType = CalculationType.KPI, Order = 1, Workspace = new Workspace() };

            DynamicKPI dynamicKPI = new() { CalculationStep = step, CalculationString = "W1_mm + 2", ConcatCalculationString = "W1_mm,+,2", OutputColumn = "result1", InputColumns = "W1_mm" };
            step.Calculations.Add(dynamicKPI);

            ICollection<CalculationStep> request = [step];

            var result = component.Parse(request);

            Assert.True(result);
        }
        // NA-XX Test invalid input and calculation
        [Fact]
        public void TestCompositeWithInvalidInput()
        {
            CalculationFactory calculationFactory = new CalculationFactory();
            ICalculationComponent component = calculationFactory.CreateCalculationValidators();
            KPIParser kPIParser = new KPIParser();
            CalculationStep step = new() { CalculationType = CalculationType.KPI, Order = 1, Workspace = new Workspace() };

            ICollection<CalculationStep> request = [step];

            var result = component.Parse(request);

            Assert.False(result);
        }

        [Fact]
        public void TestCompositeWithInvalidInput2()
        {
            CalculationFactory calculationFactory = new CalculationFactory();
            ICalculationComponent component = calculationFactory.CreateCalculationValidators();
            KPIParser kPIParser = new KPIParser();
            CalculationStep step = new() { CalculationType = CalculationType.KPI, Order = 1, Workspace = new Workspace() };

            DynamicKPI dynamicKPI = new() { CalculationStep = step, CalculationString = "W1_mm +", ConcatCalculationString = "W1_mm,+,", OutputColumn = "result1", InputColumns = "W1_mm" };
            step.Calculations.Add(dynamicKPI);

            ICollection<CalculationStep> request = [step];

            var result = component.Parse(request);

            Assert.False(result);
        }
    }
}

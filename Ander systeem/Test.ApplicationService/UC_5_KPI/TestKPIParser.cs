using Leap.ApplicationServices.AppGeneralServices.CalculationValidators;
using Leap.Domain.Domain.Calculations;
using Leap.Domain.Domain.Workspaces;

namespace Test.ApplicationService.UC_5_KPI
{
    // NOTE: THIS DOES NOT VALIDATE IF COLUMN EXISTS IN THE DATA.
    public class TestKPIParser
    {
        // Test invalid calculation starting with a operator
        [Fact]
        public void TestInvalidCalculationStartingWithAOperator()
        {
            KPIParser kPIParser = new KPIParser();
            CalculationStep step = new() { CalculationType = CalculationType.KPI, Order = 1, Workspace = new Workspace() };

            DynamicKPI dynamicKPI = new() { CalculationStep = step, CalculationString = " +W1_mm + 2", ConcatCalculationString = "+,W1_mm,+,2", OutputColumn = "result1", InputColumns = "W1_mm" };
            step.Calculations.Add(dynamicKPI);

            ICollection<CalculationStep> list = [step];

            bool result = kPIParser.Parse(list);
            Assert.False(result);
        }
        // Test invalid calculation ending with a operator
        [Fact]
        public void TestInvalidCalculationEndingWithAOperator()
        {
            KPIParser kPIParser = new KPIParser();
            CalculationStep step = new() { CalculationType = CalculationType.KPI, Order = 1, Workspace = new Workspace() };

            DynamicKPI dynamicKPI = new() { CalculationStep = step, CalculationString = " W1_mm + 2 +", ConcatCalculationString = "W1_mm,+,2,+", OutputColumn = "result1", InputColumns = "W1_mm" };
            step.Calculations.Add(dynamicKPI);

            ICollection<CalculationStep> list = [step];
            bool result = kPIParser.Parse(list);
            string actualError = kPIParser.ReturnError();
            Assert.False(result);
            Assert.Equal("An calculation should not start or end with a operator.", actualError);
        }
        // Test invalid calculation with two operators right besides eachother
        [Fact]
        public void TestInvalidCalculationWithTwoOperatorsBesidesEachother()
        {
            KPIParser kPIParser = new KPIParser();
            CalculationStep step = new() { CalculationType = CalculationType.KPI, Order = 1, Workspace = new Workspace() };

            DynamicKPI dynamicKPI = new() { CalculationStep = step, CalculationString = "W1_mm + + 2", ConcatCalculationString = "SA-W-1_d-voeg_mm,+,+,2", OutputColumn = "result1", InputColumns = "W1_mm" };
            step.Calculations.Add(dynamicKPI);

            ICollection<CalculationStep> list = [step];
            bool result = kPIParser.Parse(list);
            string actualError = kPIParser.ReturnError();
            Assert.False(result);
            Assert.Equal("No two operators or columns after eachother.", actualError);
        }
        // Test invalid calculation with two columns right besides eachother
        [Fact]
        public void TestInvalidCalculationWithTwoColumnsBesidesEachother()
        {
            KPIParser kPIParser = new KPIParser();
            CalculationStep step = new() { CalculationType = CalculationType.KPI, Order = 1, Workspace = new Workspace() };

            DynamicKPI dynamicKPI = new() { CalculationStep = step, CalculationString = "W1_mm W1_mm + 2", ConcatCalculationString = "W1_mm,W1_mm,+,2", OutputColumn = "result1", InputColumns = "W1_mm" };
            step.Calculations.Add(dynamicKPI);

            ICollection<CalculationStep> list = [step];
            bool result = kPIParser.Parse(list);
            string actualError = kPIParser.ReturnError();
            Assert.Equal("No two operators or columns after eachother.", actualError);
            Assert.False(result);
        }

        [Fact]
        public void TestValidCalculationWithTwoColumnsBesidesEachother()
        {
            KPIParser kPIParser = new KPIParser();
            CalculationStep step = new() { CalculationType = CalculationType.KPI, Order = 1, Workspace = new Workspace() };

            DynamicKPI dynamicKPI = new() { CalculationStep = step, CalculationString = "W1_mm + 2", ConcatCalculationString = "W1_mm,+,2", OutputColumn = "result1", InputColumns = "W1_mm" };
            step.Calculations.Add(dynamicKPI);

            ICollection<CalculationStep> list = [step];
            bool result = kPIParser.Parse(list);
            Assert.True(result);
        }
    }
}

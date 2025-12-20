using Leap.Domain.Domain.Calculations;
using Leap.Domain.Domain.Workspaces;

namespace Test.Leap.Domain
{
    public class TestDynamicKPI
    {
        [Fact]
        public void TestConcatCalculationStringToCollection()
        {
            // Arrange
            List<string> validationCollection = ["Column1", "+", "Column2"];
            List<string> validationInputPutCollection = ["Column1", "Column2"];
            string outputColumnName = "CustomColumn1";
            string concatedString = "Column1,+,Column2";
            string calculationString = "Column1+Column2";
            string inputColumns = "Column1,Column2";
            string calculationType = "KPI";
            Guid workspaceGuid = Guid.NewGuid();

            Workspace workspace = new() { WorkspaceGuid = workspaceGuid };
            CalculationStep step = new() { Workspace = workspace, Order = 1, CalculationType = CalculationType.KPI, Calculations = [], CalculationStepId = 1, WorkspaceGuid = workspaceGuid };
            DynamicKPI dynamicKPI = new()
            {
                CalculationStep = step,
                CalculationString = calculationString,
                CalculationType = calculationType,
                ConcatCalculationString = concatedString,
                OutputColumn = outputColumnName,
                InputColumns = inputColumns
            };
            // Act
            var result = dynamicKPI.GetCalculationArray();
            var resultInputColumns = dynamicKPI.GetInputColumns();

            // Assert
            Assert.Equal(validationCollection, result);
            Assert.Equal(validationInputPutCollection, resultInputColumns);
        }
    }
}

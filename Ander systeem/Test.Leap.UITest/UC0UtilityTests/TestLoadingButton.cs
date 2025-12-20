using Bunit;
using LeapDataScienceTool.Common.Components.Input;

namespace Test.Leap.UITest.UC0UtilityTests
{
    public class TestLoadingButton : TestContext
    {

        private void TestMethod()
        {
            Console.WriteLine();
        }
        // N-6 User experience
        [Fact]
        public async Task TestIfButtonPressReturnsToTheSameStateAsBefore()
        {
            var LoadingButton = RenderComponent<LoadingButton>(parameters => parameters
                .Add(cp => cp.ButtonName, "Test button")
                .Add(p => p.TriggerMethod, TestMethod)
            );
            var Instance = LoadingButton.Instance;
            Assert.False(Instance.EnableButton());
            await Instance.Press();
            Assert.False(Instance.EnableButton());
        }

        // N-6 User experience
        [Fact]
        public void TestIfAdditionalBooleanDisablesButton()
        {
            var LoadingButton = RenderComponent<LoadingButton>(parameters => parameters
                .Add(cp => cp.ButtonName, "Test button")
                .Add(p => p.TriggerMethod, TestMethod)
                .Add(p => p.ExternalCondition, true)
            );

            var Instance = LoadingButton.Instance;
            Assert.True(Instance.EnableButton());
        }
    }
}

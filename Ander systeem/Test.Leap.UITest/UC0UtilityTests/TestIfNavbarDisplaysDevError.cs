using Bunit;
using Bunit.TestDoubles;
using LeapDataScienceTool.Layout;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Interop;
using Test.Leap.UITest.ExtensionMethods;

namespace Test.Leap.UITest.UC0UtilityTests
{
    public class TestIfNavbarDisplaysDevError : TestContext
    {
        [Fact]
        public void TestIfNavbarDisplaysAlertIfAppIsInDevelopment()
        {

            // Arrange - Set up the development environment
            // Arrange - Register MudBlazor services
            JSInterop.Setup<BoundingClientRect>("mudElementRef.getBoundingClientRect", _ => true);
            Services.RegisterUIComponents();
            Services.AddSingleton<IWebAssemblyHostEnvironment>(new FakeWebAssemblyHostEnvironment { Environment = "Development" });
            // Act - Render the component
            var componentUnderTest = RenderComponent<NavMenu>(parameters =>
            parameters.Add(p => p.Body, new RenderFragment(builder => { builder.AddContent(0, ""); }))
        )
            ;

            // Assert - Verify that MudAlert is displayed
            var mudAlert = componentUnderTest.FindComponent<MudAlert>();
            Assert.NotNull(mudAlert);
            Assert.Equal(Severity.Warning, mudAlert.Instance.Severity);
            Assert.Equal(Variant.Filled, mudAlert.Instance.Variant);
        }
    }
}

using Bunit;
using Leap.ApplicationServices.DTO.ModelDTO;
using LeapDataScienceTool.Components.ModelingProcess.ModelAlgorithms;
using LeapDataScienceTool.ProgramSetup;
using MudBlazor;
using MudBlazor.Services;
using Test.Leap.UITest.ExtensionMethods;

namespace Test.Leap.UITest.UC2_ModelTrainingConfig
{
    public class TestModelParameterForms : TestContext
    {
        // BZ-26
        [Fact]
        public void TestLoadingSVMForm()
        {
            Services.RegisterUIComponents();
            Services.RegisterRuntimeClasses();
            JSInterop.RegisterPopOverSetup(Services);
            Services.AddMudServices();

            ModelConfigDTO dto = new()
            {
                ParentWorkspaceGuid = Guid.NewGuid()
            };
            RenderComponent<MudPopoverProvider>();
            var SVMComponent = RenderComponent<SVMConfig>(param =>
            {
                param.Add(c => c.ModelConfigDTO, dto);
            });

            var choices = SVMComponent.Instance.KernelChoice;

            Assert.Equal(["rbf", "sigmoid", "poly", "linear"], choices);
            Assert.IsType<SVMDTO>(dto.AlgorithmParameterDTO);
        }

        // BZ-25
        [Fact]
        public void TestLoadingLineairForm()
        {
            // Assert
            Services.RegisterUIComponents();
            Services.RegisterRuntimeClasses();
            JSInterop.RegisterPopOverSetup(Services);
            Services.AddMudServices();

            ModelConfigDTO dto = new()
            {
                ParentWorkspaceGuid = Guid.NewGuid()
            };
            // Act
            RenderComponent<MudPopoverProvider>();
            var component = RenderComponent<LinearRegressionConfig>(param =>
            {
                param.Add(c => c.ModelConfigDTO, dto);
            });

            var njobs = component.Instance.LinearRegressionDTO.NJobs;

            // Assert
            Assert.Equal(3, njobs);
            Assert.IsType<LinearRegressionDTO>(dto.AlgorithmParameterDTO);
        }
    }
}

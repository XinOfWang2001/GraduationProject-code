using Bunit;
using Leap.ApplicationServices.DTO.DataConfig;
using Leap.ApplicationServices.DTO.DataProcessDTO;
using LeapDataScienceTool.Common.Services;
using LeapDataScienceTool.Components.DataSourceProcess;
using LeapDataScienceTool.PageManagers;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor;

namespace Test.Leap.UITest.DataSourceConfiguration
{
    public class DataSourceComponentTest : TestContext
    {
        private readonly Mock<IDialogService> _dialogService;
        private readonly Mock<IWorkspaceManager> _workspaceManager;
        private readonly Mock<ISnackbar> _snackbarService;

        public DataSourceComponentTest()
        {
            _dialogService = new Mock<IDialogService>();
            _workspaceManager = new Mock<IWorkspaceManager>();
            _snackbarService = new Mock<ISnackbar>();

        }
        public DataExtractConfigDTO GetDummyDTO()
        {
            DataExtractConfigDTO dto = new DataExtractConfigDTO()
            {
                WorkspaceId = new Guid("a86ff674-ae5a-472a-9479-aaacb5f5ce9e"),
                StartDate = new DateTime(2024, 11, 11),
                EndDate = new DateTime(2024, 12, 1),
                DataSource = new DataSourceDTO() { DataSourceId = 2, Name = "IWA_Data" },
                SensorsSelected = new List<SensorDTO> { new SensorDTO { Id = 1, Name = "C-1" } },
                ValueTypesSelected = new List<ValueTypeDTO> { new ValueTypeDTO { Id = 1, Name = "Temp" } },
                ProjectDTO = new ProjectSourceDTO() { Id = 1, HumanReadableName = "KTYE_Project", Guid = new Guid("77d3c0ea-91b5-4e6f-9e1e-f2937edfd167"), Name = "KTYE_Project_name" },
            };
            return dto;
        }

        // Non functional: NA-27
        [Fact]
        public void TestIfNonExistentDataConfigShowsSelectText()
        {
            Services.AddSingleton(_dialogService.Object);
            Services.AddSingleton(_workspaceManager.Object);
            Services.AddSingleton(_snackbarService.Object);
            Services.AddSingleton<ResponseService>();
            // Act: Render the component
            var cut = RenderComponent<DataSourceComponent>();
            var instance = cut.Instance;

            Assert.NotNull(instance);
            Assert.Equal("Selecteer databron", instance.Name);
        }

        // Non functional: NA-28
        [Fact]
        public void TestIfExistentDataConfigShowsSelectText()
        {
            Services.AddSingleton(_dialogService.Object);
            Services.AddSingleton(_workspaceManager.Object);
            Services.AddSingleton(_snackbarService.Object);
            Services.AddSingleton<ResponseService>();
            var dto = GetDummyDTO();
            // Act: Render the component
            var cut = RenderComponent<DataSourceComponent>(parameter => parameter
            .Add(par => par.DataSourceConfig, dto));

            var instance = cut.Instance;

            Assert.NotNull(instance);
            Assert.Equal("KTYE_Project", instance.Name);
        }

    }
}

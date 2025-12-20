using Bunit;
using Leap.ApplicationServices.DTO.Workspace;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using LeapDataScienceTool.API;
using LeapDataScienceTool.Components.Workspace;
using LeapDataScienceTool.Pages;
using LeapDataScienceTool.ProgramSetup;
using LeapDataScienceTool.Services.Proxies;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor;
using Test.Leap.UITest.ExtensionMethods;

namespace Test.Leap.UITest.UC1_DataSourceConfig
{
    public class TestWorkspaceOverview : TestContext
    {
        private readonly Mock<IMudDialogInstance> _dialogInstance;

        public TestWorkspaceOverview()
        {
            _dialogInstance = new Mock<IMudDialogInstance>();
        }

        [Fact]
        public void TestWorkspaceLoad()
        {
            var mockServerAPI = new Mock<IServerAPI>();
            var serverAPI = mockServerAPI.Object;
            Services.RegisterUIComponents();
            Services.AddSingleton(serverAPI);
            Services.AddSingleton<IWorkspaceService, WorkspaceProxyService>();
            IEnumerable<WorkspaceConfigDTO> workspaces = [
                new () {WorkspaceName = "Test 1" }, new (){ WorkspaceName = "Test 2"}];
            mockServerAPI.Setup(api => api.GetAll<WorkspaceConfigDTO>(It.IsAny<string>())).ReturnsAsync(workspaces);

            var workspaceOverview = RenderComponent<WorkspaceOverview>();
            var Instance = workspaceOverview.Instance;
            Assert.True(Instance.IsSuccesfullyLoaded);
            Assert.True(Instance.HasLoadedIn);
        }

        [Fact]
        public async Task TestConfirmation()
        {
            var workspace = new WorkspaceConfigDTO() { WorkspaceName = "Pipeline test" };
            Services.RegisterUIComponents();
            Services.RegisterCustomUIServices();
            var mockServerAPI = new Mock<IServerAPI>();
            var serverAPI = mockServerAPI.Object;
            Services.AddSingleton(serverAPI);
            Services.AddSingleton<IWorkspaceService, WorkspaceProxyService>();
            mockServerAPI.Setup(api => api.Post<WorkspaceConfigDTO>(It.IsAny<string>(), workspace)).ReturnsAsync(workspace);
            var WorkspaceForm = RenderComponent<WorkspaceCreationDialog>(param =>
            {
                param.Add(cp => cp.workspaceConfig, workspace);
                param.Add(cp => cp.DialogInstance, _dialogInstance.Object);
            });

            var Instance = WorkspaceForm.Instance;

            await Instance.Confirm();

            Assert.Equal("", Instance.Error);
        }

        [Fact]
        public async Task TestConfirmationFailure()
        {
            var workspace = new WorkspaceConfigDTO() { WorkspaceName = "Pipeline test" };
            Services.RegisterUIComponents();
            var mockServerAPI = new Mock<IServerAPI>();
            var serverAPI = mockServerAPI.Object;
            Services.AddSingleton(serverAPI);
            Services.AddSingleton<IWorkspaceService, WorkspaceProxyService>();
            mockServerAPI.Setup(api => api.Post<WorkspaceConfigDTO>(It.IsAny<string>(), workspace)).ReturnsAsync((WorkspaceConfigDTO?)null);
            var WorkspaceForm = RenderComponent<WorkspaceCreationDialog>(param =>
            {
                param.Add(cp => cp.workspaceConfig, workspace);

            });

            var Instance = WorkspaceForm.Instance;

            await Instance.Confirm();

            Assert.Equal("Opslaan is mislukt", Instance.Error);
        }
    }
}

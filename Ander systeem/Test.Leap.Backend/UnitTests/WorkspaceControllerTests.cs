using Leap.ApplicationServices.DTO.Workspace;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using Leap.ApplicationServices.Interfaces.Repositories;
using Leap.Domain.Domain.Workspaces;
using LeapDataScienceAPI.Controllers;
using LeapDataScienceAPI.Services.Proxies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Test.Leap.Backend.UnitTests
{
    public class WorkspaceControllerTests
    {
        public readonly WorkspaceController controller;
        public readonly WorkspaceService WorkspaceService;
        public readonly Mock<IWorkspaceRepository> workspaceRepository;
        public readonly Mock<IDataExtractRepository> dataExtractRepository;
        public readonly Mock<IModelService> modelProxyService;
        public readonly Mock<IModelOperationService> modelOperationService;
        public readonly Mock<ICalculationRepository> MockCalculationRepository;

        public WorkspaceControllerTests()
        {
            dataExtractRepository = new Mock<IDataExtractRepository>();
            modelProxyService = new Mock<IModelService>();
            workspaceRepository = new Mock<IWorkspaceRepository>();
            modelOperationService = new Mock<IModelOperationService>();
            MockCalculationRepository = new Mock<ICalculationRepository>();
            WorkspaceService = new WorkspaceService(workspaceRepository.Object, modelProxyService.Object, modelOperationService.Object, MockCalculationRepository.Object);
            controller = new(WorkspaceService);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        // Test if unfound Workspace returns bad request
        // N-A-42
        [Fact]
        public async Task TestIfUnknownWorkspaceReturnsNull()
        {
            workspaceRepository.Setup(wr => wr.Get(It.IsAny<Guid>())).Returns((Workspace?)null);
            var result = await controller.GetOne(It.IsAny<Guid>());

            Assert.Null(result);
        }

        // Test if unsuccesfull Workspace creation returns bad request
        // N-A-43
        [Fact]
        public async Task TestIfInvalidRegistrationWorkspaceReturnsError()
        {
            WorkspaceConfigDTO workspace = new WorkspaceConfigDTO() { WorkspaceName = "test" };
            workspaceRepository.Setup(wr => wr.Create(It.IsAny<Workspace>())).Throws(new Exception());
            var result = await controller.Post(workspace);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}

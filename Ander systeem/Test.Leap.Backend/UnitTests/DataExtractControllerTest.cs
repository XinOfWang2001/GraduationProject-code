using Leap.ApplicationServices.AppGeneralServices.DataExtractDTOInput;
using Leap.ApplicationServices.DTO.DataConfig;
using Leap.ApplicationServices.DTO.DataProcessDTO;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using Leap.ApplicationServices.Interfaces.Creational;
using Leap.ApplicationServices.Interfaces.Repositories;
using Leap.Domain.Domain.DataConfig;
using Leap.Domain.Domain.DataSource;
using Leap.Domain.Domain.Workspaces;
using LeapDataScienceAPI.Controllers;
using LeapDataScienceAPI.Services.Proxies;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Test.Leap.Backend.UnitTests
{
    public class DataExtractControllerTest
    {
        private readonly Mock<IDataExtractRepository> mockDataExtract;
        private readonly Mock<IProjectRepository> mockProjectRepository;
        private readonly Mock<IWorkspaceRepository> mockWorkspaceRepository;
        private readonly IDataExtractService dataSourceServiceHandler;
        private readonly IDataExtractValidatorFactory factory;
        private readonly DataExtractController controller;

        public DataExtractControllerTest()
        {
            mockProjectRepository = new Mock<IProjectRepository>();
            mockWorkspaceRepository = new Mock<IWorkspaceRepository>();
            mockDataExtract = new Mock<IDataExtractRepository>();
            factory = new ExtractDTOInputFactory();
            dataSourceServiceHandler = new DataExtractService(mockWorkspaceRepository.Object, mockDataExtract.Object, mockProjectRepository.Object);
            controller = new(dataSourceServiceHandler, mockDataExtract.Object, factory);
        }

        private static DataExtractConfigDTO CompleteDTO()
        {
            DataExtractConfigDTO dto = new()
            {
                WorkspaceId = new Guid("a86ff674-ae5a-472a-9479-aaacb5f5ce9e"),
                ProcessId = new Guid("75FD8C8E-74AC-44BD-A6FC-3A069251C743"),
                StartDate = new DateTime(2024, 11, 11),
                EndDate = new DateTime(2024, 12, 1),
                DataSource = new DataSourceDTO() { DataSourceId = 2 },
                SensorsSelected = [new SensorDTO { Id = 1, Name = "C-1" }],
                ValueTypesSelected = [new ValueTypeDTO { Id = 1, Name = "Temp" }],
                ProjectDTO = new ProjectSourceDTO() { Id = 1, HumanReadableName = "KT", Guid = new Guid("77d3c0ea-91b5-4e6f-9e1e-f2937edfd167"), Name = "KT" },
            };
            return dto;
        }
        // N-A-41, (N-9)
        [Fact]
        public async Task TestIfUnfoundWorkspaceReturnBadRequest()
        {
            // Arrange
            var dto = CompleteDTO();
            mockWorkspaceRepository.Setup(wr => wr.Get(It.IsAny<Guid>())).Returns((Workspace?)null);
            mockProjectRepository.Setup(pr => pr.Get(It.IsAny<Guid>())).Returns((Project?)new Project() { HumanReadableName = "Test project", Name = "test" });

            // Act
            var result = await controller.Post(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        // A-46a
        [Fact]
        public async Task TestIfUnfoundEntitiesReturnBadRequestAtUpdate()
        {
            // Arrange
            var dto = CompleteDTO();
            mockWorkspaceRepository.Setup(wr => wr.Get(It.IsAny<Guid>())).Returns(new Workspace() { Name = "TestWorkspace" });
            mockProjectRepository.Setup(pr => pr.Get(It.IsAny<Guid>())).Returns((Project?)null);
            var processId = new Guid("75FD8C8E-74AC-44BD-A6FC-3A069251C743");

            // Act
            var result = await controller.Put(processId, dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        // A-46b
        [Fact]
        public async Task TestIfUnfoundProjectReturnBadRequestAtCreate()
        {
            // Arrange
            var dto = CompleteDTO();
            mockWorkspaceRepository.Setup(wr => wr.Get(It.IsAny<Guid>())).Returns(new Workspace() { Name = "TestWorkspace" });
            mockProjectRepository.Setup(pr => pr.Get(It.IsAny<Guid>())).Returns((Project?)null);
            var processId = new Guid("75FD8C8E-74AC-44BD-A6FC-3A069251C743");

            // Act
            var result = await controller.Put(processId, dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void TestInvalidInputReturnsError()
        {
            mockDataExtract.Setup(r => r.Get(It.IsAny<Guid>())).Returns((DataExtracter?)null);
            var result = controller.Get(It.IsAny<Guid>());
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}

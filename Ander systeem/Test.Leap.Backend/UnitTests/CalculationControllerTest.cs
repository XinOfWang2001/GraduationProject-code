using Leap.ApplicationServices.AppGeneralServices.CalculationValidators;
using Leap.ApplicationServices.DTO.Calculations;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using Leap.ApplicationServices.Interfaces.Repositories;
using Leap.Domain.Domain.Calculations;
using Leap.Domain.Domain.Workspaces;
using LeapDataScienceAPI.Controllers;
using LeapDataScienceAPI.Services.Proxies;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Test.Leap.Backend.UnitTests
{
    // This test is different from the file in IntegrationTests folder, because this will only handle error handling
    public class CalculationControllerTest
    {
        private Mock<IWorkspaceRepository> _workspaceRepositoryMock;
        private Mock<ICalculationRepository> _calculationRepositoryMock;

        public CalculationControllerTest()
        {
            _workspaceRepositoryMock = new Mock<IWorkspaceRepository>();
            _calculationRepositoryMock = new Mock<ICalculationRepository>();
        }
        // NA-XX Invalid inputvalidation general.
        [Fact]
        public async Task CalculationControllerWithInvalidInput()
        {
            // Arrange
            string ExpectedError = "Amount of calculations should be at least 1\n";
            Guid WorkspaceGuid = new Guid("324d5a66-ae3c-43a3-9e39-01c37ba4600e");
            KPIDTO kpi1 = new KPIDTO() { CalculationString = "KT * 2", InputColumns = ["KT"], OutputColumn = "KT_times2", OperationsList = ["KT", "*"] };
            CalculationStepDTO steps = new CalculationStepDTO()
            {
                Order = 1,
                CalculationType = CalculationType.KPI,
                Calculations = new[] { kpi1 }
            };
            CalculationStepDTO stepAnother50 = new CalculationStepDTO()
            {
                Order = 2,
                CalculationType = CalculationType.KPI,
                Calculations = []
            };

            IEnumerable<CalculationStepDTO> StepsDTO = [steps, stepAnother50];
            _workspaceRepositoryMock.Setup(ws => ws.Get(It.IsAny<Guid>())).Returns(new Workspace());
            CalculationWriteDTO RequestDTO = new CalculationWriteDTO() { Steps = StepsDTO, WorkspaceGuid = WorkspaceGuid };
            IWorkspaceRepository workspaceRepository = _workspaceRepositoryMock.Object;
            ICalculationRepository calculationRepository = _calculationRepositoryMock.Object;
            CalculationFactory factory = new CalculationFactory();
            ICalculationService service = new CalculationService(workspaceRepository, calculationRepository, factory);
            CalculationsController controller = new(service);

            // Act
            var result = await controller.Overwrite(RequestDTO);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            BadRequestObjectResult value = (BadRequestObjectResult)result.Result;
            CalculationWriteDTO actualMessage = (CalculationWriteDTO)value.Value;
            Assert.Equal(ExpectedError, actualMessage.Message);
        }

        // NA-XX Invalid database operation
        [Fact]
        public async Task CalculationControllerWithDatabaseError()
        {
            string ExpectedError = "Calculation storage went wrong. Please contact with technical specialist to investigate the issue.";
            Guid WorkspaceGuid = new Guid("324d5a66-ae3c-43a3-9e39-01c37ba4600e");
            KPIDTO kpi1 = new KPIDTO() { CalculationString = "KT * 2", InputColumns = ["KT"], OutputColumn = "KT_times2", OperationsList = ["KT", "*", "2"] };
            KPIDTO kpi2 = new KPIDTO() { CalculationString = "KT_mm + 50", InputColumns = ["KT_mm"], OutputColumn = "KT_plus50", OperationsList = ["KT_mm", "+", "50"] };
            CalculationStepDTO steps = new CalculationStepDTO()
            {
                Order = 1,
                CalculationType = CalculationType.KPI,
                Calculations = new[] { kpi1, kpi2 }
            };
            CalculationStepDTO stepAnother50 = new CalculationStepDTO()
            {
                Order = 2,
                CalculationType = CalculationType.KPI,
                Calculations = new[] { kpi2 }
            };

            IEnumerable<CalculationStepDTO> StepsDTO = [steps, stepAnother50];

            CalculationWriteDTO RequestDTO = new CalculationWriteDTO() { Steps = StepsDTO, WorkspaceGuid = WorkspaceGuid };
            _workspaceRepositoryMock.Setup(ws => ws.Get(It.IsAny<Guid>())).Returns(new Workspace());
            _calculationRepositoryMock.Setup(ws => ws.Overwrite(It.IsAny<Guid>(), It.IsAny<IEnumerable<CalculationStep>>())).Throws(new IOException(ExpectedError));
            IWorkspaceRepository workspaceRepository = _workspaceRepositoryMock.Object;
            ICalculationRepository calculationRepository = _calculationRepositoryMock.Object;
            CalculationFactory factory = new CalculationFactory();
            ICalculationService service = new CalculationService(workspaceRepository, calculationRepository, factory);
            CalculationsController controller = new(service);

            // Act
            var result = await controller.Overwrite(RequestDTO);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            BadRequestObjectResult value = (BadRequestObjectResult)result.Result;
            CalculationWriteDTO actualMessage = (CalculationWriteDTO)value.Value;
            Assert.Equal(ExpectedError, actualMessage.Message);
        }
    }
}

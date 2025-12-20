using Leap.ApplicationServices.DTO.Workspace;
using LeapDataScienceTool.API;
using Moq;
using System.Net;
using Test.Leap.UITest.ExtensionMethods;

namespace Test.Leap.UITest.UC1_DataSourceConfig
{
    public class TestServerAPI
    {
        [Fact]
        public async Task TestPositivePost()
        {

            // Arrange
            var expectedObject = new WorkspaceConfigDTO { WorkshopId = 1, WorkspaceName = "TEST" };
            var client = HttpsMockMethods.GetDummyHttpClient(expectedObject, HttpStatusCode.OK);

            var serverApi = new ServerAPI(client);

            var result = await serverApi.Post<WorkspaceConfigDTO>(It.IsAny<string>(), It.IsAny<WorkspaceConfigDTO>());

            Assert.IsType<WorkspaceConfigDTO>(result);
        }

        [Fact]
        public async Task TestPostNull()
        {

            // Arrange
            var expectedObject = new WorkspaceConfigDTO { WorkshopId = 1, WorkspaceName = "TEST" };
            var client = HttpsMockMethods.GetDummyHttpClient(null, HttpStatusCode.BadRequest);

            var serverApi = new ServerAPI(client);

            var result = await serverApi.Post<WorkspaceConfigDTO>(It.IsAny<string>(), It.IsAny<WorkspaceConfigDTO>());

            Assert.Null(result);
        }

        [Fact]
        public async Task TestPutNull()
        {

            // Arrange
            var expectedObject = new WorkspaceConfigDTO { WorkshopId = 1, WorkspaceName = "TEST" };
            var client = HttpsMockMethods.GetDummyHttpClient(null, HttpStatusCode.BadRequest);

            var serverApi = new ServerAPI(client);

            var result = await serverApi.Put<WorkspaceConfigDTO>(It.IsAny<string>(), expectedObject);

            Assert.Null(result);
        }

        [Fact]
        public async Task TestPutPositive()
        {

            // Arrange
            var expectedObject = new WorkspaceConfigDTO { WorkshopId = 1, WorkspaceName = "TEST" };
            var client = HttpsMockMethods.GetDummyHttpClient(expectedObject, HttpStatusCode.OK);

            var serverApi = new ServerAPI(client);

            var result = await serverApi.Put<WorkspaceConfigDTO>(It.IsAny<string>(), It.IsAny<WorkspaceConfigDTO>());

            Assert.IsType<WorkspaceConfigDTO>(result);
        }


        [Fact]
        public async Task TestGetNull()
        {

            // Arrange
            var expectedObject = new WorkspaceConfigDTO { WorkshopId = 1, WorkspaceName = "TEST" };
            var client = HttpsMockMethods.GetDummyHttpClient(null, HttpStatusCode.BadRequest);

            var serverApi = new ServerAPI(client);

            var result = await serverApi.Get<WorkspaceConfigDTO>(It.IsAny<string>());

            Assert.Null(result);
        }

        [Fact]
        public async Task TestGetPositive()
        {
            // Arrange
            var expectedObject = new WorkspaceConfigDTO { WorkshopId = 1, WorkspaceName = "TEST" };
            var client = HttpsMockMethods.GetDummyHttpClient(expectedObject, HttpStatusCode.OK);

            var serverApi = new ServerAPI(client);

            var result = await serverApi.Get<WorkspaceConfigDTO>(It.IsAny<string>());

            Assert.IsType<WorkspaceConfigDTO>(result);
        }

        [Fact]
        public async Task TestGetAllNull()
        {

            // Arrange
            var expectedObject = new WorkspaceConfigDTO { WorkshopId = 1, WorkspaceName = "TEST" };
            List<WorkspaceConfigDTO> expectedCollection = new List<WorkspaceConfigDTO>() { expectedObject };
            var client = HttpsMockMethods.GetDummyHttpClient(null, HttpStatusCode.BadRequest);

            var serverApi = new ServerAPI(client);

            var result = await serverApi.GetAll<WorkspaceConfigDTO>(It.IsAny<string>());

            Assert.Empty(result);
        }

        [Fact]
        public async Task TestGetAllPositive()
        {
            // Arrange
            var expectedObject = new WorkspaceConfigDTO { WorkshopId = 1, WorkspaceName = "TEST" };
            List<WorkspaceConfigDTO> expectedCollection = new List<WorkspaceConfigDTO>() { expectedObject };
            var client = HttpsMockMethods.GetDummyHttpClient(expectedCollection, HttpStatusCode.OK);

            var serverApi = new ServerAPI(client);

            var result = await serverApi.GetAll<WorkspaceConfigDTO>(It.IsAny<string>());

            Assert.IsType<List<WorkspaceConfigDTO>>(result);
        }
    }
}

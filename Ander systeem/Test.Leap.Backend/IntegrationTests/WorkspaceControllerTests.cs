using Infra.Data.DatabaseContext;
using Infra.Data.DataSeeder;
using Leap.ApplicationServices.DTO.Workspace;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Test.Leap.Backend.Fixtures;

namespace Test.Leap.Backend.IntegrationTests
{
    public class WorkspaceControllerTests : IClassFixture<LeapDSAPIAppFactory>, IAsyncLifetime
    {
        private readonly IServiceScope _scope;
        private readonly LeapDSDBContext _leapDSDBContext;
        private readonly TestDataSeeder testDataSeeder;
        private readonly HttpClient _httpClient;

        public WorkspaceControllerTests(LeapDSAPIAppFactory leapDSAPIAppFactory)
        {
            _scope = leapDSAPIAppFactory.Services.CreateScope();
            _leapDSDBContext = _scope.ServiceProvider.GetRequiredService<LeapDSDBContext>();
            _httpClient = leapDSAPIAppFactory.CreateClient();
            testDataSeeder = new TestDataSeeder(_leapDSDBContext);
        }

        [Fact]
        public async Task TestCreationWorkspace()
        {
            WorkspaceConfigDTO workspaceConfigDTO = new WorkspaceConfigDTO()
            {
                WorkspaceName = "Testov"
            };

            var response = await _httpClient.PostAsJsonAsync("api/Workspace", workspaceConfigDTO);
            var deserialized = await response.Content.ReadFromJsonAsync<WorkspaceConfigDTO>();

            var responseRegistration = await _httpClient.GetAsync($"api/Workspace/{deserialized.WorkspaceGuid}");
            var result = await responseRegistration.Content.ReadFromJsonAsync<WorkspaceConfigDTO>();

            Assert.Equal("Testov", result.WorkspaceName);
        }

        [Fact]
        public async Task TestDeletionWorkspace()
        {
            Guid workspaceGuid = new("1B63B626-BBB9-44A2-B465-FD5BE9166A69");
            var response = await _httpClient.DeleteAsync($"api/Workspace/{workspaceGuid}");
            Assert.True(response.IsSuccessStatusCode);
        }

        public Task InitializeAsync()
        {
            return testDataSeeder.SeedWorkshops();
        }

        public Task DisposeAsync()
        {
            testDataSeeder.DeleteData();
            return Task.CompletedTask;
        }
    }
}

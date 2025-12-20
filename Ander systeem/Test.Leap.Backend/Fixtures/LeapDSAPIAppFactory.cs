using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Infra.Data.DatabaseContext;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Test.Leap.Backend.Fixtures
{
    public class LeapDSAPIAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private const string Database = "master";
        private const string Username = "sa";
        private const string Password = "yourStrong(!)Password";
        private const ushort MsSqlPort = 1433;

        private readonly IContainer _mssqlContainer;

        public LeapDSAPIAppFactory()
        {
            _mssqlContainer = new ContainerBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPortBinding(MsSqlPort, true)
                .WithEnvironment("ACCEPT_EULA", "Y")
                .WithEnvironment("SQLCMDUSER", Username)
                .WithEnvironment("SQLCMDPASSWORD", Password)
                .WithEnvironment("MSSQL_SA_PASSWORD", Password)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MsSqlPort))
                .Build();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var host = _mssqlContainer.Hostname;
            var port = _mssqlContainer.GetMappedPublicPort(MsSqlPort);

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<LeapDSDBContext>));
                // Add fake webapi App for test purposes.
                services.AddScoped<IPreviewDataService, DummyPreviewDataService>();
                services.AddDbContext<LeapDSDBContext>(options => options.UseSqlServer($"Server={host},{port};Database={Database};User Id={Username};Password={Password};TrustServerCertificate=True"));
            });
        }


        public async Task InitializeAsync()
        {
            await _mssqlContainer.StartAsync();
        }

        public new async Task DisposeAsync()
        {
            await _mssqlContainer.DisposeAsync();
        }
    }
}

using Infra.Data.ImplRepositories;
using Leap.ApplicationServices.Interfaces.Repositories;
using Leap.Domain.Domain.DataSource;

namespace LeapDataScienceAPI.ProgramSetup
{
    public static class RepositorySetup
    {
        public static void ConfigureRepositories(this IServiceCollection services)
        {
            // Register repositories here
            services.AddScoped<IDataSourceRepo<SwecoDataSource>, DataSourceRepository>();
            services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IDataExtractRepository, DataExtracterRepository>();
            services.AddScoped<IModelConfigRepository, ModelConfigRepository>();
            services.AddScoped<IModelStorageRepository, ModelStorageRepository>();
            services.AddScoped<ICalculationRepository, CalculationRepository>();
        }
    }
}

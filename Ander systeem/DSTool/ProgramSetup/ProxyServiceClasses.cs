using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using LeapDataScienceTool.API;
using LeapDataScienceTool.PageManagers;
using LeapDataScienceTool.Services;
using LeapDataScienceTool.Services.Proxies;

namespace LeapDataScienceTool.ProgramSetup
{
    public static class ProxyServiceClasses
    {
        public static void RegisterProxyServices(this IServiceCollection services)
        {
            services.AddScoped<IServerAPI, ServerAPI>();
            services.AddScoped<IWorkspaceService, WorkspaceProxyService>();
            services.AddScoped<IDataSourceService, DataSourceProxyService>();
            services.AddScoped<IMonitorDataService, SwecoDataSourceService>();
            services.AddScoped<IDataExtractService, DataExtractProxyService>();
            services.AddScoped<IWorkspaceManager, WorkspaceManager>();
            services.AddScoped<ICalculationService, CalculationProxyService>();
            services.AddScoped<IPreviewDataService, PreviewDataProxyService>();
            services.AddScoped<IModelService, ModelConfigProxyService>();
            services.AddScoped<IModelOperationService, ModelOperationProxyService>();
        }
    }
}

using Leap.ApplicationServices.AppGeneralServices.CalculationValidators;
using Leap.ApplicationServices.AppGeneralServices.DataExtractDTOInput;
using Leap.ApplicationServices.AppGeneralServices.ExternalServices;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using Leap.ApplicationServices.Interfaces.Creational;
using Leap.ApplicationServices.Interfaces.ExternalServiceAPI;
using LeapDataScienceAPI.Services.Proxies;

namespace LeapDataScienceAPI.ProgramSetup
{
    public static class ServiceProxyClassSetup
    {
        public static void RegisterProxyServices(this IServiceCollection services)
        {
            services.AddScoped<IWorkspaceService, WorkspaceService>();
            services.AddScoped<IDataSourceService, DataSourceService>();
            services.AddScoped<IDataExtractValidatorFactory, ExtractDTOInputFactory>();
            services.AddScoped<IPreviewDataService, PreviewDataService>();
            services.AddScoped<IPythonFacadeService, LeapFastDSAPIService>();
            services.AddScoped<IDataExtractService, DataExtractService>();
            services.AddScoped<IModelService, ModelConfigService>();
            services.AddScoped<IModelOperationService, ModelOperationService>();
            services.AddScoped<ICalculationService, CalculationService>();
            services.AddScoped<CalculationFactory>();
        }
    }
}

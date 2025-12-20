using LeapDataScienceTool.Common.Services;

namespace LeapDataScienceTool.ProgramSetup
{
    public static class MudblazorServices
    {
        public static void RegisterCustomUIServices(this IServiceCollection services)
        {
            services.AddScoped<ResponseService>();
        }
    }
}

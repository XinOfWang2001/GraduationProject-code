using Bunit;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using LeapDataScienceTool.Common.Services;
using LeapDataScienceTool.PageManagers;
using LeapDataScienceTool.ProgramSetup;
using LeapDataScienceTool.Services.Proxies;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace Test.Leap.UITest.ExtensionMethods
{
    public static class SetupClasses
    {
        public static void RegisterUIComponents(this IServiceCollection services)
        {

            services.RegisterRuntimeClasses();
            services.AddMudServices();
            services.AddMudBlazorDialog();
            services.AddMudBlazorSnackbar();
            services.AddMudBlazorResizeListener();
            services.AddMudBlazorScrollManager();
            services.AddMudBlazorPointerEventsNoneService();
            services.AddMudLocalization();
            services.AddScoped<ResponseService>();
            services.AddScoped<IWorkspaceManager, WorkspaceManager>();
            services.AddScoped<IModelOperationService, ModelOperationProxyService>();
        }

        public static void RegisterPopOverSetup(this BunitJSInterop jSInterop, IServiceCollection services)
        {
            jSInterop.SetupVoid("mudPopover.initialize", "mud-popover-provider", 0, 24);
            jSInterop.SetupVoid("mudElementRef.addOnBlurEvent", _ => true);
            jSInterop.SetupVoid("mudKeyInterceptor.connect", _ => true);
            jSInterop.SetupVoid("mudDragAndDrop.initDropZone", _ => true);
            jSInterop.Setup<int>("mudpopoverHelper.countProviders");
            services.AddMudPopoverService();
        }
    }
}

using LeapDataScienceTool;
using LeapDataScienceTool.ProgramSetup;
using LeapDataScienceTool.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Moet generieker geschreven worden. Bijvoorbeeld voor productie URL.
string FrontendApi = builder.HostEnvironment.BaseAddress;
string BackendApi = string.Empty;

// Vereist een omgevingsvariabel
BackendApi = builder.Configuration["BACKEND_API"];

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(FrontendApi) });
builder.Services.AddHttpClient("ServerClient", config =>
{
    config.BaseAddress = new Uri(BackendApi);
    config.Timeout = new TimeSpan(1, 30, 00);
    config.DefaultRequestHeaders.Clear();
});

// PACKAGE HTTPCLientFactory pattern moet hier toegepast worden.
builder.Services.RegisterRuntimeClasses();
builder.Services.RegisterProxyServices();
builder.Services.RegisterCustomUIServices();
builder.Services.AddScoped<IAlgorithmComponentBuilder, AlgorithmBuilder>();

builder.Services.AddMudServices();

await builder.Build().RunAsync();

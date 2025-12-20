using Infra.Data.DatabaseContext;
using Core.ApplicationServices.AppGeneralServices.ExternalServices;
using Core.ApplicationServices.Interfaces.Creational;
using Core.ApplicationServices.Interfaces.ExternalServiceAPI;
using DSAPI.ProgramSetup;
using DSAPI.Services.BuilderAndMappers.ModelBuilders;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

string SmpUrl = builder.Configuration.GetValue("SMPAPIBaseUrl", "None");
string LeapPythonUrl = builder.Configuration.GetValue<string>("LeapFastDSAPI");
string ClientApp = builder.Configuration.GetValue<string>("ClientApp", "None");
string AllowAnySpecificOrigin = "_AllowSpecificOrigins";
string AllowLocalOrigin = "_LocalOrigin";

string ConnectionString = builder.Configuration.GetValue("SQLServerConnectionString", "Data Source=localhost;Initial Catalog=LeapDataDB;User ID=sa;Password=YourStrong!Passw0rd;Trust Server Certificate=True");

builder.Services.AddDbContext<LeapDSDBContext>(options =>
{
    options.UseSqlServer(ConnectionString);
});

builder.Services.AddHttpClient("IWA_Server", config =>
{
    config.BaseAddress = new Uri(SmpUrl);
    config.Timeout = new TimeSpan(0, 0, 45);
    config.DefaultRequestHeaders.Clear();
});

builder.Services.AddHttpClient("Leap_PythonService", config =>
{
    // Timeout of 30 minutes max.
    config.BaseAddress = new Uri(LeapPythonUrl);
    config.Timeout = new TimeSpan(1, 30, 0);
    config.DefaultRequestHeaders.Clear();
});

builder.Services.ConfigureRepositories();
builder.Services.RegisterRuntimeClasses();
builder.Services.RegisterProxyServices();

builder.Services.AddScoped<ISwecoWebServices<IWAWebService>, IWAWebService>();
builder.Services.AddScoped<IModelConfigBuilder, ModelConfigBuilder>();
builder.Services.AddScoped<DatabaseInitializer, DatabaseInitializer>();
// Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowLocalOrigin,
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
});
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Alleen voor ontwikkelingsdoeleinden zal dit van toepassing zijn.
    app.UseCors(AllowLocalOrigin);
    app.MapOpenApi();
    // Alleen in lokale omgevingen zullen migraties uitgevoerd worden
    // In productie zullen deze twee stappen via CI/CD apart gedaan worden.
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetService<DatabaseInitializer>();
    await context.InitializeAsync();
}
else
{
    app.UseCors(options =>
    {
        options
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Used as entry point for integration tests.
public partial class Program { }
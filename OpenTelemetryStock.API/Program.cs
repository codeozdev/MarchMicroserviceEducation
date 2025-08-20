using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args: args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddOpenTelemetry().WithTracing(traceProviderBuilder =>
{
    traceProviderBuilder.ConfigureResource(resource =>
    {
        resource.AddService(serviceName: "OpenTelemetryStock.API", serviceVersion: "1.0.0");
    });

    traceProviderBuilder.AddAspNetCoreInstrumentation();
    traceProviderBuilder.AddHttpClientInstrumentation();
    traceProviderBuilder.AddEntityFrameworkCoreInstrumentation(efcoreOptions =>
    {
        efcoreOptions.SetDbStatementForStoredProcedure = true;
        efcoreOptions.SetDbStatementForText = true;
    });

    traceProviderBuilder.AddSource("AppActivitySource");

    traceProviderBuilder.AddOtlpExporter(otlpOptions =>
    {
        otlpOptions.Endpoint = new Uri(uriString: "http://localhost:4318/v1/traces");
        otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
    });

    traceProviderBuilder.AddConsoleExporter();
}).WithLogging(logProviderBuilder =>
{
    logProviderBuilder.ConfigureResource(x =>
    {
        x.AddService(serviceName: "OpenTelemetryStock.API", serviceVersion: "1.0.0");
    });

    logProviderBuilder.AddConsoleExporter();
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseAuthorization();

app.MapControllers();

app.Run();
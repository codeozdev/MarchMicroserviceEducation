using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetryOrder.API.Models;
using OpenTelemetryOrder.API.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args: args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Entity Framework Context ekleme
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(name: "SqlServer")));

// ikinci microservisin ayaga kalktigi alan
builder.Services.AddHttpClient<StockService>(x => { x.BaseAddress = new Uri(uriString: "http://localhost:5293"); });

builder.Services.AddOpenTelemetry().WithTracing(traceProviderBuilder =>
{
    // Jaeger UI'da görebilmek için servis adı ve versiyonu ekleniyor
    traceProviderBuilder.ConfigureResource(resource =>
    {
        resource.AddService(
            serviceName: "OpenTelemetryOrder.API", // servis ismi
            serviceVersion: "1.0.0" // servis versiyonu
        );
    });

    // ASP.NET Core uygulamasındaki HTTP request/response'ları otomatik olarak yakalar
    traceProviderBuilder.AddAspNetCoreInstrumentation();

    // HttpClient üzerinden yapılan dış çağrıları otomatik olarak yakalar
    traceProviderBuilder.AddHttpClientInstrumentation();

    // Entity Framework Core üzerinden yapılan veritabanı işlemlerini yakalar
    traceProviderBuilder.AddEntityFrameworkCoreInstrumentation(efcoreOptions =>
    {
        efcoreOptions.SetDbStatementForStoredProcedure = true; // Stored Procedure sorgularını trace eder
        efcoreOptions.SetDbStatementForText = true; // Normal SQL sorgularını trace eder
    });

    // Özel olarak takip etmek istediğimiz ActivitySource'ları ekler
    traceProviderBuilder.AddSource("AppActivitySource");

    // Toplanan trace verilerini Jaeger/OTLP endpointine gönderir
    traceProviderBuilder.AddOtlpExporter(otlpOptions =>
    {
        otlpOptions.Endpoint = new Uri(uriString: "http://localhost:4318/v1/traces"); // OTLP endpoint
        otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf; // veri gönderim protokolü
    });

    // Trace verilerini aynı zamanda konsola da yazdırır (geliştirme sırasında faydalı)
    traceProviderBuilder.AddConsoleExporter();
}).WithLogging(logProviderBuilder =>
{
    // Logging tarafı için servis adı ve versiyonu ekleniyor
    logProviderBuilder.ConfigureResource(x =>
    {
        x.AddService(serviceName: "OpenTelemetryOrder.API", serviceVersion: "1.0.0");
    });

    // Logları konsola yazdırır
    logProviderBuilder.AddConsoleExporter();
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

// Iki servisin http uzerinden haberlesmesinin ornegidir bu sayede olusan verileri jaeger ui uzerinden takip ettik
// opentelemery de kuyruk istemlerini takip eden hazir bir yapi yok fakat masstransit bunu otomatik olarak ekliyor
using AMicroservice.Services;
using MassTransit;
using Polly;
using Polly.Extensions.Http;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args: args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<BusService>();
builder.Services.AddScoped<StockService>();

// Uygulama ayaga kalktiginda Masstransit otomatik olarak RabbitMQ'a baglanacak 
builder.Services.AddMassTransit(configuration =>
{
    configuration.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString(name: "RabbitMQ"));
    });
});


// Retry Pattern
var retry = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(retryCount: 5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(x: 2, y: retryAttempt)));

// Circuit Breaker Pattern (basic)
// 3 kez hata alırsa 15 saniye bekleyecek
var circuitBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 3, TimeSpan.FromSeconds(seconds: 15));


// Circuit Breaker Pattern (advanced)
// %50 hata alırsa 15 saniye bekleyecek, 3 kez hata alırsa 30 saniye bekleyecek
// var advancedCircuitBreakerPolicy = HttpPolicyExtensions
//     .HandleTransientHttpError()
//     .AdvancedCircuitBreakerAsync(failureThreshold: 0.5, TimeSpan.FromSeconds(seconds: 15), minimumThroughput: 3,
//         TimeSpan.FromSeconds(seconds: 30));


// Timeout Pattern
// 10 saniye bekleyecek
var timeoutPolicy =
    Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(seconds: 10));

// Combine policies (yukardaki 3 patterni combin yapar)
var combinedPolicy = Policy.WrapAsync(retry, circuitBreakerPolicy, timeoutPolicy);

// Bunun gibi 20 tane servisimiz olursa "reflection" kullan for ile
// StockService ekledik
// AddPolicyHandler -> methodu üzerinden combinedPolicy değişkenini argüman olarak verdik
builder.Services.AddHttpClient<StockService>(x => { x.BaseAddress = new Uri(uriString: "http://localhost:5258"); })
    .AddPolicyHandler(policy: combinedPolicy);


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}


app.UseAuthorization();

app.MapControllers();

app.Run();

// Outbox pattern

/*  Polly Kutuphanesinin kullanim seneryolari
    B ayakta degilse
    500 hatlari
    timeout hatalari 408

    bu surecler icin pattern kullanabiliriz

*/
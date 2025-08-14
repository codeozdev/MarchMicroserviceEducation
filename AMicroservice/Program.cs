using AMicroservice.Services;
using MassTransit;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<BusService>();

// Uygulama ayaga kalktiginda Masstransit otomatik olarak RabbitMQ'a baglanacak 
builder.Services.AddMassTransit(configuration =>
{
    configuration.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString(name: "RabbitMQ"));
    });
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}


app.UseAuthorization();

app.MapControllers();

app.Run();

// Outbox pattern
using BMicroservice.Consumers;
using MassTransit;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Uygulama ayaga kalktiginda Masstransit otomatik olarak RabbitMQ'a baglanacak 
builder.Services.AddMassTransit(configuration =>
{
    // gelen mesajı alacak consumer'i ekliyoruz
    configuration.AddConsumer<OrderCreatedEventConsumer>();

    configuration.UsingRabbitMq((context, cfg) =>
    {
        // retiry hata oldugunda tekrar deneme yap hala hata aliyorsan hatayi error kuyruguna gonder
        cfg.UseMessageRetry(r =>
            r.Incremental(retryLimit: 3, TimeSpan.FromSeconds(seconds: 2), TimeSpan.FromSeconds(seconds: 4)));

        cfg.Host(builder.Configuration.GetConnectionString(name: "RabbitMQ"));

        // isimlendirme kurali ilk olarak <microservice.ismi>-<event.ismi>
        cfg.ReceiveEndpoint(queueName: "b.microservice.order.created.event",
            endpoint => { endpoint.Consumer<OrderCreatedEventConsumer>(); });
    });
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

// Inbox pattern
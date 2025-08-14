using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using MassTransit;
using RabbitMQ.Client;
using Shared;

namespace AMicroservice.Services;

public class OrderService(
    IConfiguration configuration,
    ILogger<OrderService> logger,
    IPublishEndpoint publishEndpoint,
    BusService busService)
    : IOrderService
{
    public async Task CreateOrder()
    {
        string? connectionString = configuration.GetConnectionString(name: "RabbitMQ");
        ConnectionFactory factory = new() { Uri = new Uri(connectionString!) };

        await using IConnection connection = await factory.CreateConnectionAsync();
        // rabbitmq haberlesmek (tv yayinlari-kablo tek)
        // cift yonlu hem gonderebiliriz hem alabiliriz (channel uzerinden)
        await using IChannel channel = await connection.CreateChannelAsync();
        // bu channel uzerinden artik mesaj gonderebiliriz

        List<OrderItem> orderItems =
        [
            new(ProductCode: "123", Quantity: 1, Price: 100),
            new(ProductCode: "456", Quantity: 2, Price: 200)
        ];

        // message
        OrderCreatedEvent order = new(
            OrderCode: "123",
            UserId: "456",
            orderItems.ToImmutableList() // Listeye eleman eklenemez hale getirir
        );

        string orderAsJson = JsonSerializer.Serialize(value: order); // sinif -> json
        byte[] orderAsBytes = Encoding.UTF8.GetBytes(s: orderAsJson); // json -> byte array

        // exchange olusturma
        await channel.ExchangeDeclareAsync(
            exchange: "fanout-exchange", // Exchange’in RabbitMQ içinde görünecek ismi
            type: ExchangeType.Fanout, // Exchange tipi.
            durable: true, // RabbitMQ yeniden başlatıldığında bile bu exchange silinmez, varlığı korunur.
            autoDelete: false // Tüm bağlı kuyruklar ayrıldığında bu exchange otomatik olarak silinmez.
        );

        await channel.BasicPublishAsync(
            exchange: "fanout-exchange", // Hangi exchange'e mesaj gidecek
            routingKey: "", // Routing key (fanout'ta önemsiz)
            mandatory: true, // mandatory (yönlenemezse geri gelsin mi?)
            body: orderAsBytes // body Gönderilecek mesaj verisi (byte[])
        ).AsTask().ContinueWith(x =>
        {
            if (x.IsFaulted)
                logger.LogError(exception: x.Exception, message: "Order created event could not be published");
        });
    }

    public async Task CreateOrderWithMasstransit()
    {
        List<OrderItem> orderItems = new()
        {
            new OrderItem(ProductCode: "123", Quantity: 1, Price: 100),
            new OrderItem(ProductCode: "456", Quantity: 2, Price: 200)
        };

        // bu order mesaj olarak gidecek bir nesne
        OrderCreatedEvent order = new(
            OrderCode: "123",
            UserId: "456",
            orderItems.ToImmutableList() // Listeye eleman eklenemez hale getirir
        );

        // normal tanimlama
        // await publishEndpoint.Publish(order, pipe =>
        // {
        //     pipe.SetAwaitAck(true);
        //     pipe.Durable = true; // Mesajı diske yazar
        //     pipe.MessageId = NewId.NextGuid(); // header tarafinda gider
        // });


        // disarda tanimlama (kod tekrarini onlemek ayni seyleri yazmamak adina)
        await busService.PublishAsync(message: order);
    }
}
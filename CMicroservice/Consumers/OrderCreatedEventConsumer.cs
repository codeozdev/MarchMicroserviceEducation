using MassTransit;
using Shared;

namespace CMicroservice.Consumers;

public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
{
    public Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        string? version = context.Headers.Get(key: "version", defaultValue: string.Empty);
        OrderCreatedEvent message = context.Message;
        Guid? messageId = context.MessageId;

        Console.WriteLine(value: "Consumer: C Microservice");

        // throw new Exception("db error"); (retry testi icin)

        Console.WriteLine(
            $"OrderCreatedEventConsumer: {message.OrderCode} - {message.UserId} - {version} - {messageId}");

        return Task.CompletedTask;
    }
}
using MassTransit;

namespace AMicroservice.Services;

public class BusService(IPublishEndpoint publishEndpoint)
{
    public async Task PublishAsync<TMessageOrEvent>(TMessageOrEvent message)
    {
        // sonsuz retry olmamasi adina
        CancellationTokenSource cts = new();
        cts.CancelAfter(TimeSpan.FromSeconds(seconds: 30)); // 30 saniye sonra hata mesaji ver

        await publishEndpoint.Publish(message!, pipe =>
        {
            pipe.Headers.Set(key: "version", value: "1.0.0");
            pipe.SetAwaitAck(awaitAck: true);
            pipe.Durable = true;
            pipe.MessageId = NewId.NextGuid();
        }, cancellationToken: cts.Token);
    }
}
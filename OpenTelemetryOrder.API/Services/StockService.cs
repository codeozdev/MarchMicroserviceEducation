namespace OpenTelemetryOrder.API.Services;

public class StockService(HttpClient httpClient)
{
    public async Task GetStockCount()
    {
        var response = await httpClient.GetAsync(requestUri: "/api/stocks");
        response.EnsureSuccessStatusCode();
        await response.Content.ReadAsStringAsync();
    }
}

// amacimiz datayi almak degil sadece okumak
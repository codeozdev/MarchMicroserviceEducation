using AMicroservice.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace AMicroservice.Services;

public class StockService(HttpClient httpClient)
{
    public async Task<int> GetStockCount(int productId)
    {
        // İkinci GET (A Mikroservisi → B Mikroservisi)
        var response = await httpClient.GetAsync($"api/Stocks/{productId}");

        if (response.IsSuccessStatusCode)
        {
            // JSON geliyor → C# Nesnesi Dönüştürülüyor
            var responseAsContext = await response.Content.ReadFromJsonAsync<GetStockResponse>();
            return responseAsContext!.StockCount;
        }

        var responseAsError = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // neden throw attık çünkü geriye int dönüyor ondan durdurmak adına throw yazdık
        throw new Exception(message: responseAsError!.Title);
    }
}

// bir cevap almayacagiz sadece ornek olmasi adina yazdik (test ederken BMicroservisi ayaga bile kaldirmadik)
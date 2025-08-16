using AMicroservice.Services;
using Microsoft.AspNetCore.Mvc;

namespace AMicroservice.Controllers;

[Route(template: "api/[controller]")]
[ApiController]
public class OrdersController(IOrderService orderService, StockService stockService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetOrder()
    {
        await orderService.CreateOrder();
        return Ok(value: "Sent message");
    }

    [HttpGet(template: "masstransit")]
    public async Task<IActionResult> GetOrderWithMasstransit()
    {
        await orderService.CreateOrderWithMasstransit();
        return Ok(value: "Sent message");
    }

    // İlk GET (Postman → A Mikroservisi)
    [HttpGet(template: "polly")]
    public async Task<IActionResult> OrderCheck()
    {
        return Ok(await stockService.GetStockCount(productId: 2));
    }
}
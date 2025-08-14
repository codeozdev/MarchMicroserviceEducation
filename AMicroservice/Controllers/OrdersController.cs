using AMicroservice.Services;
using Microsoft.AspNetCore.Mvc;

namespace AMicroservice.Controllers;

[Route(template: "api/[controller]")]
[ApiController]
public class OrdersController(IOrderService orderService) : ControllerBase
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
}
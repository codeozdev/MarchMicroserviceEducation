using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetryOrder.API.Models;
using OpenTelemetryOrder.API.Services;

namespace OpenTelemetryOrder.API.Controllers;

[Route(template: "api/[controller]")]
[ApiController]
public class ProductsController(AppDbContext context, ILogger<ProductsController> logger, StockService stockService)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProduct()
    {
        logger.LogInformation(message: "Get Product");

        await stockService.GetStockCount();
        return Ok(context.Products.ToList());
    }

    // bu kendi urettigimiz activitysource -> traceProviderBuilder.AddSource yazarak aldigimiz yer
    [HttpPost]
    public IActionResult Get()
    {
        // ⏱️ BAŞLANGIC - Kronometre başlar (Parent Activity)
        using (var activity = ActivitySourceProvider.ActivitySource.StartActivity(name: "RequestOperation"))
        {
            // db isleminin ne kadar surede gerceklestigini izlememiz saglayacak


            const int userId = 100;

            using (var activity1 = ActivitySourceProvider.ActivitySource.StartActivity(name: "DbOperation"))
            {
                // db isleminin ne kadar surede gerceklestigini izlememiz saglayacak

                activity1.AddEvent(new ActivityEvent(name: ":db:operation:start")); // zaman damgasi eklemek icin
                activity1.SetTag(key: "userId", value: userId);
            }

            using (var activity2 = ActivitySourceProvider.ActivitySource.StartActivity(name: "QueueOperation"))
            {
                // queue operations
            }

            using (var activity3 = ActivitySourceProvider.ActivitySource.StartActivity(name: "RedisOperation"))
            {
                // redis operations
            }

            {
                return Ok(value: "Hello World");
            }
        }
    }
}
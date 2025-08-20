using Microsoft.AspNetCore.Mvc;

namespace OpenTelemetryStock.API.Controllers;

[Route(template: "api/[controller]")]
[ApiController]
public class StocksController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(value: 1);
    }
}
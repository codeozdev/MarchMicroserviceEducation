using DockerExample.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DockerExample.API.Controllers;

[Route(template: "api/[controller]")]
[ApiController]
public class ProductsController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(context.Products.ToList());
    }
}
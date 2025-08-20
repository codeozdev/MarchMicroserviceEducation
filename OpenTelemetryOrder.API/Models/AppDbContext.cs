using Microsoft.EntityFrameworkCore;

namespace OpenTelemetryOrder.API.Models;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options: options)
{
    public DbSet<Product> Products { get; set; }
}
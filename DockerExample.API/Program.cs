using DockerExample.API.Models;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args: args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(name: "SqlServer")));

var app = builder.Build();

// Staging ortaminda ayaga kaldirirsak Scalar calismayacak
if (app.Environment.IsDevelopment())
{
}

app.MapOpenApi();
app.MapScalarApiReference();


// Test endpoint'leri ekleyin
app.MapGet(pattern: "/", () => "API is running!");


app.UseAuthorization();

app.MapControllers();

app.Run();
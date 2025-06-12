using Microsoft.AspNetCore.Mvc;
using Paypal.API;
using System.Buffers.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

app.MapPost("/CreatePayment", ([FromBody] IEnumerable<ItemDto> items, IConfiguration configuration, HttpContext context) =>
{
    var baseUrl = context.Request.Host.Value;
    return new PayPalService(configuration).CreatePayment(items, baseUrl);
});

app.MapPost("/ExecutePayment", ([FromBody] ExecutePaymentDto dto, IConfiguration configuration, HttpContext context) =>
{
    return new PayPalService(configuration).ExecutePayment(dto);
});

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.Run();

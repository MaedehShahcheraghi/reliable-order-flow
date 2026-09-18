using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Order.Api.Contracts;
using Order.Infrastructure.Data;
using OrderProcessing.Contracts.Events;
using OrderEntity = Order.Domain.Entities.Order;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<OrderDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("OrderDatabase")));

builder.Services.AddMassTransit(x =>
{
       x.AddEntityFrameworkOutbox<OrderDbContext>(
        options =>
        {
            options.UsePostgres();

            options.UseBusOutbox();
    });

    x.UsingRabbitMq((_,cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("admin");
            h.Password("admin123");
        });
    });

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/orders", async ([FromBody]CreateOrderRequest request,OrderDbContext dbContext, IPublishEndpoint publishEndpoint) =>
{

    var order= new OrderEntity(request.CustomerId, request.ProductId, request.Quantity, request.TotalAmount);
    dbContext.Orders.Add(order);


    await publishEndpoint.Publish(new OrderSubmitted
    {
        OrderId = order.Id,
        CustomerId = order.CustomerId,
        ProductId = order.ProductId,
        Quantity = order.Quantity,
        TotalAmount = order.TotalAmount,
        SubmittedAtUtc = DateTime.UtcNow
    },context=> context.CorrelationId = order.Id);
    await dbContext.SaveChangesAsync();

    return Results.Accepted(
    $"/orders/{order.Id}",
    new
    {
        OrderId = order.Id,
        Status = "Submitted"
    });
});

app.MapGet("/orders/{id}", async (Guid id, OrderDbContext dbContext) =>
{
    var order = await dbContext.Orders.FindAsync(id);

    if (order is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new
    {
        order.Id,
        order.CustomerId,
        order.ProductId,
        order.Quantity,
        order.TotalAmount,
        Status = order.Status.ToString(),
        order.CreatedAtUtc
    });
});

app.MapGet("/orders", async (OrderDbContext dbContext) =>
{
    var orders = await dbContext.Orders.ToListAsync();

    var result = orders.Select(order => new
    {
        order.Id,
        order.CustomerId,
        order.ProductId,
        order.Quantity,
        order.TotalAmount,
        Status = order.Status.ToString(),
        order.CreatedAtUtc
    });

    return Results.Ok(result);
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

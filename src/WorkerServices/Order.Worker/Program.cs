using MassTransit;
using Order.Infrastructure.Data;
using Order.Worker.Consumers;
using Order.Worker;
using OrderProcessing.Contracts.Events;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDbContext<OrderDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("OrderDatabase")));
builder.Services.AddMassTransit(x =>
{

    x.AddConsumer<PaymentCompletedConsumer>();
    x.UsingRabbitMq((context, configuration) =>
    {
        configuration.Host("localhost", "/", h =>
        {
            h.Username("admin");
            h.Password("admin123");
        });

        configuration.ReceiveEndpoint("order-payment-completed", e => e.ConfigureConsumer<PaymentCompletedConsumer>(context));
    });
});
var host = builder.Build();
host.Run();

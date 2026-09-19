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
    x.AddConsumer<PaymentFaultConsumer>();
    x.AddEntityFrameworkOutbox<OrderDbContext>(options =>
    {
        options.UsePostgres();
        options.UseBusOutbox();
    });

    x.UsingRabbitMq((context, configuration) =>
    {
        configuration.Host("localhost", "/", h =>
        {
            h.Username("admin");
            h.Password("admin123");
        });

        configuration.ReceiveEndpoint("order-payment-completed", e =>
        {
            e.UseEntityFrameworkOutbox<OrderDbContext>(context);
            e.UseMessageRetry(r => r.Exponential(3, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3)));
            e.ConfigureConsumer<PaymentCompletedConsumer>(context);
        });

        configuration.ReceiveEndpoint("payment-fault-queue", e =>
        {
            e.UseEntityFrameworkOutbox<OrderDbContext>(context);
            e.ConfigureConsumer<PaymentFaultConsumer>(context);
        });
    });
});
var host = builder.Build();
host.Run();

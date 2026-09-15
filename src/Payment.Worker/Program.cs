using MassTransit;
using Payment.Worker.Consumers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderSubmittedConsumer>();
    x.UsingRabbitMq((context, configuration) =>
    {
        configuration.Host("localhost", "/", h =>
        {
            h.Username("admin");
            h.Password("admin123");
        });

        configuration.ReceiveEndpoint("payment-submitted-queue", e => e.ConfigureConsumer<OrderSubmittedConsumer>(context));
    });
});

var host = builder.Build();
host.Run();

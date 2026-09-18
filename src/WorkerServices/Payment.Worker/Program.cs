using MassTransit;
using Microsoft.EntityFrameworkCore;
using Payment.Worker.Consumers;
using Payment.Worker.Data;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddDbContext<PaymentDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("PaymentDatabase")));
builder.Services.AddMassTransit(x =>
{

    x.AddConsumer<OrderSubmittedConsumer>();
    x.AddEntityFrameworkOutbox<PaymentDbContext>(options =>
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

        configuration.ReceiveEndpoint("payment-submitted-queue", e => e.ConfigureConsumer<OrderSubmittedConsumer>(context));
    });
});

var host = builder.Build();
host.Run();

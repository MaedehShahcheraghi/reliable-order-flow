using MassTransit;
using Microsoft.EntityFrameworkCore;
using Payment.Worker.Consumers;
using Payment.Worker.Data;
using Payment.Worker.GatewaySimulation;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddDbContext<PaymentDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("PaymentDatabase")));
var schedulerEndpoint = new Uri("queue:quartz");
builder.Services.AddMassTransit(x =>
{
    x.AddMessageScheduler(schedulerEndpoint);
    x.AddConsumer<ProcessPaymentConsumer>();
    x.AddEntityFrameworkOutbox<PaymentDbContext>(options => options.UsePostgres());
    x.UsingRabbitMq((context, configuration) =>
    {

        configuration.Host("localhost", "/", h =>
        {
            h.Username("admin");
            h.Password("admin123");
        });

       configuration.UseMessageScheduler(
        schedulerEndpoint);

        configuration.ReceiveEndpoint("payment-process-queue", e =>
        {
              e.UseScheduledRedelivery(r =>
                {
                    r.Handle<TimeoutException>();
                    r.Handle<HttpRequestException>();

                    r.Intervals(
                        TimeSpan.FromSeconds(15),
                        TimeSpan.FromSeconds(30),
                        TimeSpan.FromMinutes(1));
                });

            e.UseMessageRetry(r =>
            {
                r.Handle<TimeoutException>();
                r.Handle<HttpRequestException>();

                r.Immediate(2);
            });
            
            e.UseEntityFrameworkOutbox<PaymentDbContext>(context);

             e.ConfigureConsumer<ProcessPaymentConsumer>(context);
        });
    });

});

builder.Services.AddScoped<IPaymentGateway, PaymentGateway>();

var host = builder.Build();
host.Run();

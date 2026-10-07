using MassTransit;
using Order.Infrastructure.Data;
using Order.Worker.Consumers;
using Order.Worker;
using OrderProcessing.Contracts.Events;
using Microsoft.EntityFrameworkCore;
using Order.Worker.Sagas;
using Order.Infrastructure.Sagas;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDbContext<OrderDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("OrderDatabase")));
builder.Services.AddMassTransit(x =>
{
   /* x.AddConsumer<PaymentCompletedConsumer>();
    x.AddConsumer<PaymentFaultConsumer>();
    x.AddConsumer<PaymentFailedConsumer>();
    */
    x.AddSagaStateMachine<
        OrderStateMachine,
        OrderSagaState>()
    .EntityFrameworkRepository(r =>
    {
        r.ConcurrencyMode =
            ConcurrencyMode.Pessimistic;

        r.UsePostgres();

        r.ExistingDbContext<
            OrderDbContext>();
    });
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
       configuration.ReceiveEndpoint(
         "order-saga",
          e =>
         {
                e.UseMessageRetry(
                    r => r.Immediate(3));


                e.UseEntityFrameworkOutbox<
                    OrderDbContext>(context);


                e.ConfigureSaga<
                    OrderSagaState>(context);
            });
      /* configuration.ReceiveEndpoint(
    "order-payment-events",
    e =>
    {
        e.UseEntityFrameworkOutbox<OrderDbContext>(
            context);

        e.UseMessageRetry(
            r => r.Exponential(
                3,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(30),
                TimeSpan.FromSeconds(3)));


        e.ConfigureConsumer<PaymentCompletedConsumer>(
            context);

        e.ConfigureConsumer<PaymentFailedConsumer>(
            context);
    });

        configuration.ReceiveEndpoint("payment-fault-queue", e =>
        {
            e.UseEntityFrameworkOutbox<OrderDbContext>(context);
            e.ConfigureConsumer<PaymentFaultConsumer>(context);
        });  */
    });
});
var host = builder.Build();
host.Run();

using Inventory.Worker;
using MassTransit;
using Inventory.Worker.Consumers;
using Inventory.Worker.Data;
using Microsoft.EntityFrameworkCore;
using OrderProcessing.Contracts.Commands;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDbContext<InventoryDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("InventoryDatabase")));
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ReserveInventoryConsumer>();
    x.AddConsumer<ReleaseInventoryConsumer>();
    x.AddEntityFrameworkOutbox<InventoryDbContext>(options =>
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

        // configuration.ReceiveEndpoint("inventory-submitted-queue", e => e.ConfigureConsumer<OrderSubmittedConsumer>(context));
          configuration.ReceiveEndpoint(
            "inventory-reserve",
            e =>
            {
                e.UseMessageRetry(r =>
                {
                    r.Handle<DbUpdateConcurrencyException>();

                    r.Immediate(3);
                });


                e.UseEntityFrameworkOutbox<
                    InventoryDbContext>(context);


                e.ConfigureConsumer<
                    ReserveInventoryConsumer>(context);
            });

              configuration.ReceiveEndpoint(
            "inventory-release",
            e =>
            {
                e.UseMessageRetry(r =>
                {
                    r.Handle<DbUpdateConcurrencyException>();

                    r.Immediate(3);
                });


                e.UseEntityFrameworkOutbox<
                    InventoryDbContext>(context);


                e.ConfigureConsumer<
                    ReleaseInventoryConsumer>(context);
            });
    });
});

var host = builder.Build();
host.Run();

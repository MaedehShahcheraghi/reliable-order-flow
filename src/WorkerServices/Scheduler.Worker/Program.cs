using MassTransit;
using Quartz;

var builder = Host.CreateApplicationBuilder(args);

var quartzConnectionString =
    builder.Configuration
        .GetConnectionString("QuartzDatabase")
    ?? throw new InvalidOperationException(
        "QuartzDatabase connection string is missing.");

        builder.Services.AddQuartz(q =>
{
    q.SchedulerName =
        "ReliableOrderFlow-Scheduler";

    q.SchedulerId =
        "AUTO";

    q.UseDefaultThreadPool(tp => tp.MaxConcurrency = 10);

    q.UsePersistentStore(store =>
    {
        store.UseProperties = true;

        store.RetryInterval =
            TimeSpan.FromSeconds(15);


        store.UsePostgres(
            quartzConnectionString);


        store.UseSystemTextJsonSerializer();


        store.UseClustering(cluster =>
        {
            cluster.CheckinInterval =
                TimeSpan.FromSeconds(10);

            cluster.CheckinMisfireThreshold =
                TimeSpan.FromSeconds(20);
        });
    });
});



builder.Services.AddMassTransit(x =>
{
    x.AddQuartzConsumers(options => options.QueueName = "quartz");


    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(
            "localhost",
            "/",
            h =>
            {
                h.Username("admin");
                h.Password("admin123");
            });


        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);


var host = builder.Build();
host.Run();

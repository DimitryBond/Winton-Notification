using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using SmsService.Consumers;
using SmsService.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IConnection>(_ =>
        {
            var hostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
            var userName = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "admin";
            var password = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "admin123";

            var factory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = userName,
                Password = password
            };

            return factory.CreateConnection();
        });

        services.AddSingleton<SmsSender>();
        services.AddHostedService<SmsNotificationConsumer>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

await host.RunAsync();



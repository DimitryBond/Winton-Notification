using System.Text;
using System.Text.Json;
using Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PushService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PushService.Consumers;

public class PushNotificationConsumer : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly ILogger<PushNotificationConsumer> _logger;
    private readonly PushSender _sender;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    private const string QueueName = "notifications.push";

    public PushNotificationConsumer(
        ILogger<PushNotificationConsumer> logger,
        PushSender sender,
        IConnection connection)
    {
        _logger = logger;
        _sender = sender;
        _connection = connection;

        _channel = _connection.CreateModel();
        _channel.QueueDeclare(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (_, args) =>
        {
            var body = args.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);

            NotificationMessage? message = null;

            try
            {
                message = JsonSerializer.Deserialize<NotificationMessage>(json, JsonOptions);

                if (message is null)
                {
                    _logger.LogWarning("Received null or invalid NotificationMessage from queue {Queue}", QueueName);
                    _channel.BasicNack(args.DeliveryTag, multiple: false, requeue: false);
                    return;
                }

                _logger.LogInformation(
                    "[PUSH] Message received: Id={Id}, Recipient={Recipient}, RetryCount={RetryCount}",
                    message.Id,
                    message.Recipient,
                    message.RetryCount);

                await _sender.SendAsync(message.Recipient, message.Message);

                _channel.BasicAck(args.DeliveryTag, multiple: false);

                _logger.LogInformation(
                    "[PUSH] Message processed successfully: Id={Id}",
                    message.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[PUSH] Error while processing message Id={Id}. NACK with requeue.",
                    message?.Id);

                _channel.BasicNack(args.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _channel.BasicConsume(
            queue: QueueName,
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation("PushNotificationConsumer started and listening on queue {Queue}", QueueName);

        return Task.CompletedTask;
    }
}



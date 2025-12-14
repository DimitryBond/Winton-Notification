using System.Text;
using System.Text.Json;
using Contracts;
using Gateway.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Gateway.Services;

// Сервис для публикации уведомлений в RabbitMQ
public class RabbitMQService : IRabbitMQService, IDisposable
{
    private readonly RabbitMQOptions _options;
    private readonly ILogger<RabbitMQService> _logger;
    private IConnection? _connection;
    private IModel? _channel;
    private readonly object _lock = new();

    public RabbitMQService(IOptions<RabbitMQOptions> options, ILogger<RabbitMQService> logger)
    {
        _options = options.Value;
        _logger = logger;
        InitializeConnection();
    }

    private void InitializeConnection()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Создаем очереди для каждого типа уведомления
            _channel.QueueDeclare(queue: "notifications.email", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueDeclare(queue: "notifications.sms", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueDeclare(queue: "notifications.push", durable: true, exclusive: false, autoDelete: false);

            _logger.LogInformation("RabbitMQ connection established");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize RabbitMQ connection");
            throw;
        }
    }

    public Task<bool> PublishNotificationAsync(NotificationMessage message)
    {
        try
        {
            lock (_lock)
            {
                if (_channel == null || _channel.IsClosed)
                {
                    InitializeConnection();
                }

                var queueName = message.Type switch
                {
                    NotificationType.Email => "notifications.email",
                    NotificationType.Sms => "notifications.sms",
                    NotificationType.Push => "notifications.push",
                    _ => throw new ArgumentException($"Unknown notification type: {message.Type}")
                };

                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                var properties = _channel!.CreateBasicProperties();
                properties.Persistent = true;

                _channel.BasicPublish(
                    exchange: "",
                    routingKey: queueName,
                    basicProperties: properties,
                    body: body);

                _logger.LogInformation(
                    "Notification published to queue {QueueName}, Id: {NotificationId}",
                    queueName, message.Id);

                return Task.FromResult(true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish notification {NotificationId}", message.Id);
            return Task.FromResult(false);
        }
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}


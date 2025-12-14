using Contracts;

namespace Gateway.Services;

// Интерфейс для публикации сообщений в RabbitMQ
public interface IRabbitMQService
{
    Task<bool> PublishNotificationAsync(NotificationMessage message);
}


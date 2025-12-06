namespace Contracts;

// Модель сообщения для очереди (RabbitMQ/Kafka)
// Gateway создает это сообщение из NotificationRequest и отправляет в очередь
public class NotificationMessage
{
    // Уникальный идентификатор уведомления
    public Guid Id { get; set; }

    // Тип канала отправки
    public NotificationType Type { get; set; }

    // Получатель уведомления
    public string Recipient { get; set; } = string.Empty;

    // Тема уведомления (для email)
    public string? Subject { get; set; }

    // Текст сообщения
    public string Message { get; set; } = string.Empty;

    // Дополнительные метаданные (JSON строка)
    public string? Metadata { get; set; }

    // Время создания запроса
    public DateTime CreatedAt { get; set; }

    // Количество попыток отправки (для ретраев)
    public int RetryCount { get; set; } = 0;
}


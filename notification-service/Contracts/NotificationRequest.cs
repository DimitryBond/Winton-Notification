namespace Contracts;

// Модель запроса на отправку уведомления (приходит в Gateway через REST API)
public class NotificationRequest
{
    // Уникальный идентификатор уведомления
    public Guid Id { get; set; } = Guid.NewGuid();

    // Тип канала отправки
    public NotificationType Type { get; set; }

    // Получатель уведомления (email, телефон, device token и т.д.)
    public string Recipient { get; set; } = string.Empty;

    // Тема уведомления (для email)
    public string? Subject { get; set; }

    // Текст сообщения
    public string Message { get; set; } = string.Empty;

    // Дополнительные метаданные (JSON строка)
    public string? Metadata { get; set; }

    // Время создания запроса
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


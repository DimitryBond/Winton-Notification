namespace Contracts;

// Результат обработки уведомления
// Микросервис возвращает это в Gateway после обработки (для сохранения в БД и логирования)
public class NotificationResult
{
    // Идентификатор уведомления
    public Guid NotificationId { get; set; }

    // Статус обработки
    public NotificationStatus Status { get; set; }

    // Сообщение об ошибке (если есть)
    public string? ErrorMessage { get; set; }

    // Время обработки
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;

    // Количество попыток отправки
    public int RetryCount { get; set; }
}


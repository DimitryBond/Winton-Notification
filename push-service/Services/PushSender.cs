namespace PushService.Services;

public class PushSender
{
    public Task SendAsync(string recipient, string message)
    {
        Console.WriteLine($"Sending PUSH to {recipient}: {message}");
        // Имитация задержки отправки
        return Task.Delay(500);
    }
}



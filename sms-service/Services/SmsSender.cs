namespace SmsService.Services;

public class SmsSender
{
    public Task SendAsync(string recipient, string message)
    {
        Console.WriteLine($"Sending SMS to {recipient}: {message}");
        // Имитация задержки отправки
        return Task.Delay(500);
    }
}



namespace EmailService.Services;

public class EmailSender
{
    public Task SendAsync(string recipient, string message)
    {
        Console.WriteLine($"Sending EMAIL to {recipient}: {message}");
        // Имитация задержки отправки
        return Task.Delay(500);
    }
}



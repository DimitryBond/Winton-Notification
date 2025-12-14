using Contracts;
using Gateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        IRabbitMQService rabbitMQService,
        ILogger<NotificationsController> logger)
    {
        _rabbitMQService = rabbitMQService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var message = new NotificationMessage
            {
                Id = request.Id,
                Type = request.Type,
                Recipient = request.Recipient,
                Subject = request.Subject,
                Message = request.Message,
                Metadata = request.Metadata,
                CreatedAt = request.CreatedAt,
                RetryCount = 0
            };

            var success = await _rabbitMQService.PublishNotificationAsync(message);

            if (success)
            {
                _logger.LogInformation(
                    "Notification accepted and queued. Id: {NotificationId}, Type: {Type}, Recipient: {Recipient}",
                    request.Id, request.Type, request.Recipient);

                return Accepted(new
                {
                    id = request.Id,
                    status = "queued",
                    message = "Notification has been queued for processing"
                });
            }

            return StatusCode(500, new
            {
                error = "Failed to queue notification"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing notification request {NotificationId}", request.Id);
            return StatusCode(500, new
            {
                error = "Internal server error"
            });
        }
    }
}


using Microsoft.AspNetCore.Mvc;
using NotificationService.Interfaces;
using NotificationService.Models;

namespace NotificationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpPost("send")]
    public async Task<ActionResult<NotificationResponse>> SendNotification([FromBody] NotificationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _notificationService.SendNotificationAsync(request);
            
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification");
            return StatusCode(500, new NotificationResponse
            {
                Success = false,
                Message = "Internal server error",
                Errors = { "An unexpected error occurred while sending the notification" }
            });
        }
    }

    [HttpPost("email")]
    public async Task<ActionResult<NotificationResponse>> SendEmail([FromBody] EmailNotificationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _notificationService.SendEmailAsync(
                request.TemplateName, 
                request.Parameters, 
                request.ToEmails);
            
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email");
            return StatusCode(500, new NotificationResponse
            {
                Success = false,
                Message = "Internal server error",
                Errors = { "An unexpected error occurred while sending the email" }
            });
        }
    }

    [HttpPost("sms")]
    public async Task<ActionResult<NotificationResponse>> SendSms([FromBody] SmsNotificationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _notificationService.SendSmsAsync(
                request.TemplateName, 
                request.Parameters, 
                request.ToPhoneNumbers);
            
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SMS");
            return StatusCode(500, new NotificationResponse
            {
                Success = false,
                Message = "Internal server error",
                Errors = { "An unexpected error occurred while sending the SMS" }
            });
        }
    }

    [HttpGet("health")]
    public ActionResult<object> Health()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Service = "Notification Service"
        });
    }
}

public class EmailNotificationRequest
{
    public string TemplateName { get; set; } = string.Empty;
    public Dictionary<string, string> Parameters { get; set; } = new();
    public List<string> ToEmails { get; set; } = new();
}

public class SmsNotificationRequest
{
    public string TemplateName { get; set; } = string.Empty;
    public Dictionary<string, string> Parameters { get; set; } = new();
    public List<string> ToPhoneNumbers { get; set; } = new();
}

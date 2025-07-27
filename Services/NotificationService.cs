using NotificationService.Interfaces;
using NotificationService.Models;

namespace NotificationService.Services;

public class NotificationService : INotificationService
{
    private readonly NotificationConfig _config;
    private readonly EmailService _emailService;
    private readonly SmsService _smsService;
    private readonly ITemplateService _templateService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        NotificationConfig config,
        EmailService emailService,
        SmsService smsService,
        ITemplateService templateService,
        ILogger<NotificationService> logger)
    {
        _config = config;
        _emailService = emailService;
        _smsService = smsService;
        _templateService = templateService;
        _logger = logger;
    }

    public async Task<NotificationResponse> SendNotificationAsync(NotificationRequest request)
    {
        var response = new NotificationResponse();
        var errors = new List<string>();

        // Validate template exists
        if (!await _templateService.TemplateExistsAsync(request.TemplateName))
        {
            response.Success = false;
            response.Errors.Add($"Template '{request.TemplateName}' not found");
            return response;
        }

        // Determine what types of notifications to send
        var shouldSendEmail = _config.EmailEnabled && 
                             (request.Type == Models.NotificationType.Email || request.Type == Models.NotificationType.Both) &&
                             request.ToEmails.Any();

        var shouldSendSms = _config.SmsEnabled && 
                           (request.Type == Models.NotificationType.Sms || request.Type == Models.NotificationType.Both) &&
                           request.ToPhoneNumbers.Any();

        if (!shouldSendEmail && !shouldSendSms)
        {
            response.Success = false;
            response.Errors.Add("No valid notification type or recipients specified");
            return response;
        }

        // Send notifications based on type
        if (shouldSendEmail)
        {
            try
            {
                var emailResponse = await _emailService.SendEmailAsync(
                    request.TemplateName, 
                    request.Parameters, 
                    request.ToEmails);

                if (emailResponse.Success)
                {
                    response.EmailMessageId = emailResponse.EmailMessageId;
                }
                else
                {
                    errors.AddRange(emailResponse.Errors);
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Email sending failed: {ex.Message}");
                _logger.LogError(ex, "Failed to send email notification");
            }
        }

        if (shouldSendSms)
        {
            try
            {
                var smsResponse = await _smsService.SendSmsAsync(
                    request.TemplateName, 
                    request.Parameters, 
                    request.ToPhoneNumbers);

                if (smsResponse.Success)
                {
                    response.SmsMessageId = smsResponse.SmsMessageId;
                }
                else
                {
                    errors.AddRange(smsResponse.Errors);
                }
            }
            catch (Exception ex)
            {
                errors.Add($"SMS sending failed: {ex.Message}");
                _logger.LogError(ex, "Failed to send SMS notification");
            }
        }

        // Determine overall success
        response.Success = !errors.Any() && (response.EmailMessageId != null || response.SmsMessageId != null);
        response.Errors = errors;

        if (response.Success)
        {
            response.Message = "Notification sent successfully";
            _logger.LogInformation("Notification sent successfully using template {Template}", request.TemplateName);
        }
        else
        {
            response.Message = "Failed to send notification";
            _logger.LogWarning("Failed to send notification using template {Template}. Errors: {Errors}", 
                request.TemplateName, string.Join(", ", errors));
        }

        return response;
    }

    public async Task<NotificationResponse> SendEmailAsync(string templateName, Dictionary<string, string> parameters, List<string> toEmails)
    {
        if (!_config.EmailEnabled)
        {
            return new NotificationResponse
            {
                Success = false,
                Message = "Email notifications are disabled",
                Errors = { "Email notifications are disabled in configuration" }
            };
        }

        return await _emailService.SendEmailAsync(templateName, parameters, toEmails);
    }

    public async Task<NotificationResponse> SendSmsAsync(string templateName, Dictionary<string, string> parameters, List<string> toPhoneNumbers)
    {
        if (!_config.SmsEnabled)
        {
            return new NotificationResponse
            {
                Success = false,
                Message = "SMS notifications are disabled",
                Errors = { "SMS notifications are disabled in configuration" }
            };
        }

        return await _smsService.SendSmsAsync(templateName, parameters, toPhoneNumbers);
    }
}

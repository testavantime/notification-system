using NotificationService.Interfaces;
using NotificationService.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace NotificationService.Services;

public class SmsService
{
    private readonly SmsConfig _smsConfig;
    private readonly ITemplateService _templateService;
    private readonly ILogger<SmsService> _logger;

    public SmsService(SmsConfig smsConfig, ITemplateService templateService, ILogger<SmsService> logger)
    {
        _smsConfig = smsConfig;
        _templateService = templateService;
        _logger = logger;
    }

    public async Task<NotificationResponse> SendSmsAsync(string templateName, Dictionary<string, string> parameters, List<string> toPhoneNumbers)
    {
        var response = new NotificationResponse();

        try
        {
            if (string.IsNullOrEmpty(_smsConfig.AccountSid) || string.IsNullOrEmpty(_smsConfig.AuthToken))
            {
                response.Success = false;
                response.Errors.Add("SMS configuration is incomplete");
                return response;
            }

            // Initialize Twilio client
            TwilioClient.Init(_smsConfig.AccountSid, _smsConfig.AuthToken);

            // Get and process template
            var template = await _templateService.GetSmsTemplateAsync(templateName);
            var processedContent = await _templateService.ProcessTemplateAsync(template, parameters);

            var messageIds = new List<string>();

            // Send SMS to each phone number
            foreach (var phoneNumber in toPhoneNumbers)
            {
                try
                {
                    var message = await MessageResource.CreateAsync(
                        body: processedContent,
                        from: new PhoneNumber(_smsConfig.FromNumber),
                        to: new PhoneNumber(phoneNumber)
                    );

                    messageIds.Add(message.Sid);
                    _logger.LogInformation("SMS sent successfully to {PhoneNumber} using template {Template}", 
                        phoneNumber, templateName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send SMS to {PhoneNumber} using template {Template}", 
                        phoneNumber, templateName);
                    response.Errors.Add($"Failed to send SMS to {phoneNumber}: {ex.Message}");
                }
            }

            if (messageIds.Any())
            {
                response.Success = true;
                response.Message = $"SMS sent successfully to {messageIds.Count} recipients";
                response.SmsMessageId = string.Join(", ", messageIds);
            }
            else
            {
                response.Success = false;
                response.Message = "Failed to send SMS to any recipients";
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Errors.Add($"Failed to send SMS: {ex.Message}");
            _logger.LogError(ex, "Failed to send SMS using template {Template}", templateName);
        }

        return response;
    }
}

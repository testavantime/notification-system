using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using NotificationService.Interfaces;
using NotificationService.Models;

namespace NotificationService.Services;

public class EmailService
{
    private readonly EmailConfig _emailConfig;
    private readonly ITemplateService _templateService;
    private readonly ILogger<EmailService> _logger;

    public EmailService(EmailConfig emailConfig, ITemplateService templateService, ILogger<EmailService> logger)
    {
        _emailConfig = emailConfig;
        _templateService = templateService;
        _logger = logger;
    }

    public async Task<NotificationResponse> SendEmailAsync(string templateName, Dictionary<string, string> parameters, List<string> toEmails)
    {
        var response = new NotificationResponse();

        try
        {
            if (!_emailConfig.UseSsl && string.IsNullOrEmpty(_emailConfig.SmtpServer))
            {
                response.Success = false;
                response.Errors.Add("Email configuration is incomplete");
                return response;
            }

            // Get and process template
            var template = await _templateService.GetEmailTemplateAsync(templateName);
            var processedContent = await _templateService.ProcessTemplateAsync(template, parameters);

            // Create email message
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailConfig.FromName, _emailConfig.FromEmail));
            
            foreach (var toEmail in toEmails)
            {
                email.To.Add(new MailboxAddress("", toEmail));
            }

            email.Subject = GetEmailSubject(templateName);
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = processedContent
            };

            // Send email
            using var client = new SmtpClient();
            await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.SmtpPort, 
                _emailConfig.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
            
            if (!string.IsNullOrEmpty(_emailConfig.Username))
            {
                await client.AuthenticateAsync(_emailConfig.Username, _emailConfig.Password);
            }

            var messageId = await client.SendAsync(email);
            await client.DisconnectAsync(true);

            response.Success = true;
            response.Message = "Email sent successfully";
            response.EmailMessageId = messageId;

            _logger.LogInformation("Email sent successfully to {Recipients} using template {Template}", 
                string.Join(", ", toEmails), templateName);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Errors.Add($"Failed to send email: {ex.Message}");
            _logger.LogError(ex, "Failed to send email to {Recipients} using template {Template}", 
                string.Join(", ", toEmails), templateName);
        }

        return response;
    }

    private string GetEmailSubject(string templateName)
    {
        return templateName.ToLower() switch
        {
            "welcome" => "Welcome to Our Service!",
            "password-reset" => "Password Reset Request",
            "order-confirmation" => "Order Confirmation",
            _ => "Notification from Our Service"
        };
    }
}

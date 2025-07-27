using NotificationService.Interfaces;

namespace NotificationService.Services;

public class TemplateService : ITemplateService
{
    private readonly Dictionary<string, string> _emailTemplates;
    private readonly Dictionary<string, string> _smsTemplates;

    public TemplateService()
    {
        _emailTemplates = new Dictionary<string, string>
        {
            ["welcome"] = @"
                <html>
                <body>
                    <h2>Welcome to Our Service!</h2>
                    <p>Hello {{Name}},</p>
                    <p>Welcome to our platform! We're excited to have you on board.</p>
                    <p>Your account has been successfully created with email: {{Email}}</p>
                    <p>Best regards,<br/>The Team</p>
                </body>
                </html>",
            ["password-reset"] = @"
                <html>
                <body>
                    <h2>Password Reset Request</h2>
                    <p>Hello {{Name}},</p>
                    <p>You have requested a password reset for your account.</p>
                    <p>Click the link below to reset your password:</p>
                    <p><a href='{{ResetLink}}'>Reset Password</a></p>
                    <p>If you didn't request this, please ignore this email.</p>
                    <p>Best regards,<br/>The Team</p>
                </body>
                </html>",
            ["order-confirmation"] = @"
                <html>
                <body>
                    <h2>Order Confirmation</h2>
                    <p>Hello {{Name}},</p>
                    <p>Thank you for your order!</p>
                    <p><strong>Order ID:</strong> {{OrderId}}</p>
                    <p><strong>Total Amount:</strong> {{Amount}}</p>
                    <p><strong>Expected Delivery:</strong> {{DeliveryDate}}</p>
                    <p>Best regards,<br/>The Team</p>
                </body>
                </html>"
        };

        _smsTemplates = new Dictionary<string, string>
        {
            ["welcome"] = "Welcome {{Name}}! Your account has been created successfully. Welcome aboard!",
            ["password-reset"] = "Your password reset code is: {{ResetCode}}. Valid for 10 minutes.",
            ["order-confirmation"] = "Order {{OrderId}} confirmed! Total: {{Amount}}. Expected delivery: {{DeliveryDate}}.",
            ["verification"] = "Your verification code is: {{VerificationCode}}. Enter this code to verify your account."
        };
    }

    public async Task<string> GetEmailTemplateAsync(string templateName)
    {
        await Task.Delay(1); // Simulate async operation
        return _emailTemplates.TryGetValue(templateName.ToLower(), out var template) 
            ? template 
            : throw new ArgumentException($"Email template '{templateName}' not found.");
    }

    public async Task<string> GetSmsTemplateAsync(string templateName)
    {
        await Task.Delay(1); // Simulate async operation
        return _smsTemplates.TryGetValue(templateName.ToLower(), out var template) 
            ? template 
            : throw new ArgumentException($"SMS template '{templateName}' not found.");
    }

    public async Task<string> ProcessTemplateAsync(string template, Dictionary<string, string> parameters)
    {
        await Task.Delay(1); // Simulate async operation
        var processedTemplate = template;

        foreach (var parameter in parameters)
        {
            processedTemplate = processedTemplate.Replace($"{{{{{parameter.Key}}}}}", parameter.Value);
        }

        return processedTemplate;
    }

    public async Task<bool> TemplateExistsAsync(string templateName)
    {
        await Task.Delay(1); // Simulate async operation
        return _emailTemplates.ContainsKey(templateName.ToLower()) || 
               _smsTemplates.ContainsKey(templateName.ToLower());
    }
}

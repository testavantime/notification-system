namespace NotificationService.Models;

public class NotificationConfig
{
    public bool SmsEnabled { get; set; } = false;
    public bool EmailEnabled { get; set; } = false;
    public EmailConfig Email { get; set; } = new();
    public SmsConfig Sms { get; set; } = new();
}

public class EmailConfig
{
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool UseSsl { get; set; } = true;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
}

public class SmsConfig
{
    public string AccountSid { get; set; } = string.Empty;
    public string AuthToken { get; set; } = string.Empty;
    public string FromNumber { get; set; } = string.Empty;
}

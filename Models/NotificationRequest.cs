namespace NotificationService.Models;

public class NotificationRequest
{
    public string TemplateName { get; set; } = string.Empty;
    public Dictionary<string, string> Parameters { get; set; } = new();
    public List<string> ToEmails { get; set; } = new();
    public List<string> ToPhoneNumbers { get; set; } = new();
    public NotificationType Type { get; set; } = NotificationType.Both;
}

public enum NotificationType
{
    Email = 0,
    Sms = 1,
    Both = 2
}

public class NotificationResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public string? EmailMessageId { get; set; }
    public string? SmsMessageId { get; set; }
}

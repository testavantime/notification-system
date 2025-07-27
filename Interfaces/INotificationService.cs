using NotificationService.Models;

namespace NotificationService.Interfaces;

public interface INotificationService
{
    Task<NotificationResponse> SendNotificationAsync(NotificationRequest request);
    Task<NotificationResponse> SendEmailAsync(string templateName, Dictionary<string, string> parameters, List<string> toEmails);
    Task<NotificationResponse> SendSmsAsync(string templateName, Dictionary<string, string> parameters, List<string> toPhoneNumbers);
}

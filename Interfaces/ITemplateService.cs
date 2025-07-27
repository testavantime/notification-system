namespace NotificationService.Interfaces;

public interface ITemplateService
{
    Task<string> GetEmailTemplateAsync(string templateName);
    Task<string> GetSmsTemplateAsync(string templateName);
    Task<string> ProcessTemplateAsync(string template, Dictionary<string, string> parameters);
    Task<bool> TemplateExistsAsync(string templateName);
}

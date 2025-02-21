namespace Dhbw.ThesisManager.NotificationService.Services;

public interface IEmailService
{
    Task SendEmailAsync(string subject, string body, IEnumerable<string> recipients);
}

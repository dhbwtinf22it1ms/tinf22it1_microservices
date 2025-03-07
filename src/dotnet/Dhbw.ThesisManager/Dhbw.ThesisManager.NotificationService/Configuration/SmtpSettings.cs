namespace Dhbw.ThesisManager.NotificationService.Configuration;

public class SmtpSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool UseSsl { get; set; }
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
}

public class RabbitMqSettings
{
    public string Host { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ThesisExchange { get; set; } = "thesis_events";
    public string NewThesisQueue { get; set; } = "new_thesis_notifications";
}

public class NotificationSettings
{
    public List<string> NotificationRecipients { get; set; } = new();
}

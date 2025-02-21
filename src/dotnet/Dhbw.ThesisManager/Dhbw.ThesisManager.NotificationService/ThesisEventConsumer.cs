using Dhbw.ThesisManager.Domain.Events;
using Dhbw.ThesisManager.NotificationService.Configuration;
using Dhbw.ThesisManager.NotificationService.Services;
using EasyNetQ;
using Microsoft.Extensions.Options;

namespace Dhbw.ThesisManager.NotificationService;

public class ThesisEventConsumer : BackgroundService
{
    private readonly IBus _bus;
    private readonly IEmailService _emailService;
    private readonly ILogger<ThesisEventConsumer> _logger;
    private readonly NotificationSettings _notificationSettings;

    public ThesisEventConsumer(
        IOptions<RabbitMqSettings> rabbitMqSettings,
        IOptions<NotificationSettings> notificationSettings,
        IEmailService emailService,
        ILogger<ThesisEventConsumer> logger)
    {
        _emailService = emailService;
        _logger = logger;
        _notificationSettings = notificationSettings.Value;

        var connectionString = $"host={rabbitMqSettings.Value.Host};username={rabbitMqSettings.Value.Username};password={rabbitMqSettings.Value.Password}";
        _bus = RabbitHutch.CreateBus(connectionString);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _bus.PubSub.SubscribeAsync<NewThesisAdded>(
            subscriptionId: "thesis_notification_service",
            HandleNewThesisAsync,
            cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleNewThesisAsync(NewThesisAdded thesisEvent)
    {
        try
        {
            var subject = $"Neue Bachelorarbeit: {thesisEvent.Title}";
            var body = $@"
                <h2>Neue Bachelorarbeit wurde hinzugefügt</h2>
                <p><strong>Titel:</strong> {thesisEvent.Title}</p>
                <p><strong>Student:</strong> {thesisEvent.StudentName}</p>
                <p><strong>Betreuer:</strong> {thesisEvent.SupervisorName}</p>
                <p><strong>Erstellt am:</strong> {thesisEvent.CreatedAt:g}</p>
            ";

            await _emailService.SendEmailAsync(subject, body, _notificationSettings.NotificationRecipients);
            _logger.LogInformation("Notification sent for thesis {ThesisId}", thesisEvent.ThesisId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process thesis notification for {ThesisId}", thesisEvent.ThesisId);
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _bus.Dispose();
        await base.StopAsync(cancellationToken);
    }
}

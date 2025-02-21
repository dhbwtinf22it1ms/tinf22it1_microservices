using Dhbw.ThesisManager.NotificationService;
using Dhbw.ThesisManager.NotificationService.Configuration;
using Dhbw.ThesisManager.NotificationService.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("Smtp"));

builder.Services.Configure<RabbitMqSettings>(
    builder.Configuration.GetSection("RabbitMq"));

builder.Services.Configure<NotificationSettings>(
    builder.Configuration.GetSection("Notification"));

builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddHostedService<ThesisEventConsumer>();

var host = builder.Build();
host.Run();

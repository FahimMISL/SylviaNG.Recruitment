using Confluent.Kafka;
using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Enums;
using System.Text.Json;

namespace SylviaNG.Recruitment.Infrastructure.Kafka;

public class NotificationEventConsumer : BackgroundService
{
    private readonly KafkaSettings _settings;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationEventConsumer> _logger;

    public const string TOPIC_REQUISITION = "sylviang-recruitment.requisition.events";
    public const string TOPIC_JOB_POSTING = "sylviang-recruitment.job-posting.events";
    public const string TOPIC_ACCOUNT = "sylviang-recruitment.account.events";

    public NotificationEventConsumer(
        IOptions<KafkaSettings> options,
        IServiceProvider serviceProvider,
        ILogger<NotificationEventConsumer> logger)
    {
        _settings = options.Value;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (string.IsNullOrWhiteSpace(_settings.BootstrapServers))
        {
            _logger.LogInformation("Kafka BootstrapServers not configured — notification event consumer disabled.");
            return;
        }

        _logger.LogInformation("Notification event consumer starting...");

        try
        {
            await Task.Run(() =>
            {
                var config = new ConsumerConfig
                {
                    BootstrapServers = _settings.BootstrapServers,
                    GroupId = "sylviang-recruitment-notification-consumer",
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = false
                };

                using var consumer = new ConsumerBuilder<string, string>(config)
                    .SetErrorHandler((_, e) => _logger.LogError("Kafka notification consumer error: {Reason}", e.Reason))
                    .SetPartitionsAssignedHandler((c, partitions) =>
                        _logger.LogInformation("Partitions assigned: {Partitions}", string.Join(", ", partitions)))
                    .SetPartitionsRevokedHandler((c, partitions) =>
                        _logger.LogInformation("Partitions revoked: {Partitions}", string.Join(", ", partitions)))
                    .Build();

                consumer.Subscribe(new[] { TOPIC_REQUISITION, TOPIC_JOB_POSTING, TOPIC_ACCOUNT });
                _logger.LogInformation("Subscribed to notification topics on thread {ThreadId}", Environment.CurrentManagedThreadId);

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var result = consumer.Consume(TimeSpan.FromSeconds(2));
                        if (result == null || result.IsPartitionEOF) continue;

                        _logger.LogInformation("Consumed message from {Topic}, partition {Partition}, offset {Offset}",
                            result.Topic, result.Partition.Value, result.Offset.Value);

                        ProcessEventAsync(result.Topic, result.Message.Value, stoppingToken).GetAwaiter().GetResult();
                        consumer.Commit(result);
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Kafka notification consume error");
                        Thread.Sleep(5000);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing notification event");
                    }
                }

                consumer.Close();
            }, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Notification consumer crashed");
        }
    }

    private async Task ProcessEventAsync(string topic, string payload, CancellationToken ct)
    {
        _logger.LogInformation("Processing event from topic {Topic}: {Payload}", topic, payload);

        using var doc = JsonDocument.Parse(payload);
        var root = doc.RootElement;
        var action = root.GetProperty("action").GetString() ?? "";

        using var scope = _serviceProvider.CreateScope();
        var notificationService = scope.ServiceProvider.GetRequiredService<IUserNotificationService>();

        switch (topic)
        {
            case TOPIC_REQUISITION:
                await HandleRequisitionEvent(action, root, notificationService);
                break;
            case TOPIC_JOB_POSTING:
                await HandleJobPostingEvent(action, root, notificationService);
                break;
            case TOPIC_ACCOUNT:
                await HandleAccountEvent(action, root, notificationService);
                break;
        }

        _logger.LogInformation("Successfully processed {Action} event from {Topic}", action, topic);
    }

    private async Task HandleRequisitionEvent(string action, JsonElement root, IUserNotificationService svc)
    {
        var title = root.TryGetProperty("title", out var t) ? t.GetString() ?? "" : "";
        _logger.LogInformation("Handling requisition event: action={Action}, title={Title}", action, title);

        switch (action)
        {
            case "SUBMITTED":
                _logger.LogInformation("Notifying Admin role about submitted requisition");
                await svc.NotifyRoleAsync("Admin",
                    "Requisition Pending Approval",
                    $"\"{title}\" needs your review.",
                    UserNotificationTypeEnum.Warning, "/requisitions");
                break;
            case "APPROVED":
                _logger.LogInformation("Notifying HR role about approved requisition");
                await svc.NotifyRoleAsync("HR",
                    "Requisition Approved",
                    $"\"{title}\" has been approved.",
                    UserNotificationTypeEnum.Success, "/requisitions");
                break;
            case "REJECTED":
                _logger.LogInformation("Notifying HR role about rejected requisition");
                await svc.NotifyRoleAsync("HR",
                    "Requisition Rejected",
                    $"\"{title}\" has been rejected.",
                    UserNotificationTypeEnum.Error, "/requisitions");
                break;
        }
    }

    private async Task HandleJobPostingEvent(string action, JsonElement root, IUserNotificationService svc)
    {
        var title = root.TryGetProperty("title", out var t) ? t.GetString() ?? "" : "";

        if (action == "CREATED")
        {
            await svc.NotifyRoleAsync("Admin",
                "New Job Posting Created",
                $"\"{title}\" has been created.",
                UserNotificationTypeEnum.Info, "/job-postings");
        }
    }

    private async Task HandleAccountEvent(string action, JsonElement root, IUserNotificationService svc)
    {
        var keycloakUserId = root.TryGetProperty("keycloakUserId", out var k) ? k.GetString() ?? "" : "";
        var role = root.TryGetProperty("role", out var r) ? r.GetString() ?? "" : "";

        if (action == "CREATED" && !string.IsNullOrEmpty(keycloakUserId))
        {
            await svc.NotifyUserAsync(keycloakUserId,
                "Welcome to Smart Recruitment",
                $"Your {role} account is ready. Start by exploring the dashboard.",
                UserNotificationTypeEnum.Success, "/dashboard");
        }
    }
}

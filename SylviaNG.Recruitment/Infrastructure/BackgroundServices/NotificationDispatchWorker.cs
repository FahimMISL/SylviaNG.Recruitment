using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Infrastructure.Services;

namespace SylviaNG.Recruitment.Infrastructure.BackgroundServices
{
    /// <summary>
    /// Drains NotificationDispatchQueue and replays each queued DispatchAsync off the HTTP request
    /// thread. Same _serviceProvider.CreateAsyncScope() shape as ExportRequestWorker - a singleton
    /// hosted service can't hold the scoped DbContext/INotificationDispatchService itself.
    ///
    /// Reads one item at a time on purpose: sends stay serialized exactly as they were when they
    /// ran inline, so this changes when mail is sent, not how much load it puts on the SMTP host.
    /// </summary>
    public class NotificationDispatchWorker : BackgroundService
    {
        private readonly NotificationDispatchQueue _queue;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationDispatchWorker> _logger;

        public NotificationDispatchWorker(
            INotificationDispatchQueue queue,
            IServiceProvider serviceProvider,
            ILogger<NotificationDispatchWorker> logger)
        {
            // Registered as the concrete type so the worker can reach Reader without widening the
            // interface that application code depends on.
            _queue = (NotificationDispatchQueue)queue;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("NotificationDispatchWorker starting...");

            await foreach (var request in _queue.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    await using var scope = _serviceProvider.CreateAsyncScope();
                    var dispatchService = scope.ServiceProvider.GetRequiredService<INotificationDispatchService>();

                    // persistImmediately: true - there is no outer unit of work out here, and the
                    // inline payment path used to pass false and then never SaveChanges, silently
                    // discarding every NotificationLog row it created.
                    await dispatchService.DispatchAsync(
                        request.RecruitmentEvent,
                        request.PlaceholderValues,
                        request.Targets,
                        persistImmediately: true,
                        request.Attachments,
                        stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    // DispatchAsync already swallows per-recipient failures into NotificationLog
                    // rows; this only catches scope/resolution faults. Never let one bad item kill
                    // the worker - the queue would silently stop draining for the whole process.
                    _logger.LogError(ex, "Failed to dispatch queued {RecruitmentEvent} notification.", request.RecruitmentEvent);
                }
            }
        }
    }
}

using System.Threading.Channels;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Infrastructure.Services
{
    /// <summary>
    /// Singleton in-memory channel behind INotificationDispatchQueue, drained by
    /// NotificationDispatchWorker. Bounded so a stuck SMTP host can't grow the queue without limit;
    /// DropWrite (rather than Wait) keeps TryEnqueue non-blocking, since the whole point is that no
    /// HTTP request ever waits on mail.
    /// </summary>
    public class NotificationDispatchQueue : INotificationDispatchQueue
    {
        private const int Capacity = 1000;

        private readonly Channel<NotificationDispatchRequest> _channel;
        private readonly ILogger<NotificationDispatchQueue> _logger;

        public NotificationDispatchQueue(ILogger<NotificationDispatchQueue> logger)
        {
            _logger = logger;
            _channel = Channel.CreateBounded<NotificationDispatchRequest>(new BoundedChannelOptions(Capacity)
            {
                FullMode = BoundedChannelFullMode.DropWrite,
                SingleReader = true,
                SingleWriter = false
            });
        }

        public ChannelReader<NotificationDispatchRequest> Reader => _channel.Reader;

        public bool TryEnqueue(NotificationDispatchRequest request)
        {
            // Snapshot the placeholders - callers build these dictionaries locally and the worker
            // reads them on another thread, potentially after the caller's scope is gone.
            var snapshot = new NotificationDispatchRequest(
                request.RecruitmentEvent,
                new Dictionary<string, string>(request.PlaceholderValues, StringComparer.OrdinalIgnoreCase),
                request.Targets,
                request.Attachments);

            if (_channel.Writer.TryWrite(snapshot))
                return true;

            _logger.LogError(
                "Notification dispatch queue is full ({Capacity}) - dropped {RecruitmentEvent} for JobApplicationId {JobApplicationId}.",
                Capacity, request.RecruitmentEvent, request.Targets.JobApplicationId);
            return false;
        }
    }
}

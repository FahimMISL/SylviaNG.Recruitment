using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Models
{
    public class BulkNotificationRequest
    {
        public List<string> Recipients { get; set; } = new();
        public NotificationChannelEnum Channel { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}

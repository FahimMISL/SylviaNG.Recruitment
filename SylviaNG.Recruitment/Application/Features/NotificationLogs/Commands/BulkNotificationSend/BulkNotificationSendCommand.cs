using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.BulkNotificationSend
{
    public class BulkNotificationSendCommand : IRequest<BulkNotificationResult>
    {
        public BulkNotificationRequest Request { get; set; }

        public BulkNotificationSendCommand(BulkNotificationRequest request)
        {
            Request = request;
        }
    }

    public class BulkNotificationResult
    {
        public int TotalSent { get; set; }
        public int TotalFailed { get; set; }
        public List<string> FailedRecipients { get; set; } = new();
    }
}

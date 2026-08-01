using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.NotificationLogRetry
{
    public class NotificationLogRetryCommand : IRequest<NotificationLogResponse>
    {
        public long NotificationLogId { get; }

        public NotificationLogRetryCommand(long notificationLogId)
        {
            NotificationLogId = notificationLogId;
        }
    }
}

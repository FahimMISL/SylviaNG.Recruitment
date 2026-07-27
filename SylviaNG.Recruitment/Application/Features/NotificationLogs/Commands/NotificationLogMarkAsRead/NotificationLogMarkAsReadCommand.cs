using MediatR;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.NotificationLogMarkAsRead
{
    public class NotificationLogMarkAsReadCommand : IRequest<Unit>
    {
        public long NotificationLogId { get; }

        public NotificationLogMarkAsReadCommand(long notificationLogId)
        {
            NotificationLogId = notificationLogId;
        }
    }
}

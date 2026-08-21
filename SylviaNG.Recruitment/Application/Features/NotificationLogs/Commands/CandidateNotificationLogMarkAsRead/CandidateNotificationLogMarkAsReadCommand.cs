using MediatR;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.CandidateNotificationLogMarkAsRead
{
    public class CandidateNotificationLogMarkAsReadCommand : IRequest<Unit>
    {
        public long NotificationLogId { get; set; }

        public CandidateNotificationLogMarkAsReadCommand(long notificationLogId)
        {
            NotificationLogId = notificationLogId;
        }
    }
}

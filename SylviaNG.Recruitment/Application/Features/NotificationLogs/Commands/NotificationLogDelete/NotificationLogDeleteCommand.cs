using MediatR;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.NotificationLogDelete
{
    public class NotificationLogDeleteCommand : IRequest<Unit>
    {
        public long NotificationLogId { get; set; }

        public NotificationLogDeleteCommand(long notificationLogId)
        {
            NotificationLogId = notificationLogId;
        }
    }
}

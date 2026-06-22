using MediatR;

namespace SylviaNG.Recruitment.Application.Features.NotificationEvents.Commands.NotificationEventDelete
{
    public class NotificationEventDeleteCommand : IRequest<Unit>
    {
        public long NotificationEventId { get; set; }

        public NotificationEventDeleteCommand(long notificationEventId)
        {
            NotificationEventId = notificationEventId;
        }
    }
}

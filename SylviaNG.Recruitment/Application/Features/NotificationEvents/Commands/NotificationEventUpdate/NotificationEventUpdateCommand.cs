using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationEvents.Models;

namespace SylviaNG.Recruitment.Application.Features.NotificationEvents.Commands.NotificationEventUpdate
{
    public class NotificationEventUpdateCommand : IRequest<Unit>
    {
        public long NotificationEventId { get; set; }
        public NotificationEventUpdateRequest Request { get; set; }

        public NotificationEventUpdateCommand(long notificationEventId, NotificationEventUpdateRequest request)
        {
            NotificationEventId = notificationEventId;
            Request = request;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationEvents.Models;

namespace SylviaNG.Recruitment.Application.Features.NotificationEvents.Queries.NotificationEventGetById
{
    public class NotificationEventGetByIdQuery : IRequest<NotificationEventResponse>
    {
        public long NotificationEventId { get; set; }

        public NotificationEventGetByIdQuery(long notificationEventId)
        {
            NotificationEventId = notificationEventId;
        }
    }
}

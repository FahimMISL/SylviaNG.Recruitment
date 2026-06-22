using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Queries.NotificationLogGetById
{
    public class NotificationLogGetByIdQuery : IRequest<NotificationLogResponse>
    {
        public long NotificationLogId { get; set; }

        public NotificationLogGetByIdQuery(long notificationLogId)
        {
            NotificationLogId = notificationLogId;
        }
    }
}

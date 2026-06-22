using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.NotificationLogUpdate
{
    public class NotificationLogUpdateCommand : IRequest<Unit>
    {
        public long NotificationLogId { get; set; }
        public NotificationLogUpdateRequest Request { get; set; }

        public NotificationLogUpdateCommand(long notificationLogId, NotificationLogUpdateRequest request)
        {
            NotificationLogId = notificationLogId;
            Request = request;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Commands.NotificationTemplateUpdate
{
    public class NotificationTemplateUpdateCommand : IRequest<Unit>
    {
        public long NotificationTemplateId { get; set; }
        public NotificationTemplateUpdateRequest Request { get; set; }

        public NotificationTemplateUpdateCommand(long notificationTemplateId, NotificationTemplateUpdateRequest request)
        {
            NotificationTemplateId = notificationTemplateId;
            Request = request;
        }
    }
}

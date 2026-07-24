using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Queries.NotificationTemplateGetById
{
    public class NotificationTemplateGetByIdQuery : IRequest<NotificationTemplateResponse>
    {
        public long NotificationTemplateId { get; set; }

        public NotificationTemplateGetByIdQuery(long notificationTemplateId)
        {
            NotificationTemplateId = notificationTemplateId;
        }
    }
}

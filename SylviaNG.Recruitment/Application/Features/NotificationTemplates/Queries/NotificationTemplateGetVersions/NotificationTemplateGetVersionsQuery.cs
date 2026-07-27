using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Queries.NotificationTemplateGetVersions
{
    public class NotificationTemplateGetVersionsQuery : IRequest<List<NotificationTemplateVersionResponse>>
    {
        public long NotificationTemplateId { get; set; }

        public NotificationTemplateGetVersionsQuery(long notificationTemplateId)
        {
            NotificationTemplateId = notificationTemplateId;
        }
    }
}

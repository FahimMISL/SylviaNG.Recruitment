using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Queries.NotificationTemplatePreview
{
    public class NotificationTemplatePreviewQuery : IRequest<NotificationTemplatePreviewResponse>
    {
        public NotificationTemplatePreviewRequest Request { get; set; }

        public NotificationTemplatePreviewQuery(NotificationTemplatePreviewRequest request)
        {
            Request = request;
        }
    }
}

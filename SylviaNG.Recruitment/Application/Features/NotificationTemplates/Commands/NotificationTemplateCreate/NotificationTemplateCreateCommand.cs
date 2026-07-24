using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Commands.NotificationTemplateCreate
{
    public class NotificationTemplateCreateCommand : IRequest<long>
    {
        public NotificationTemplateCreateRequest Request { get; set; }

        public NotificationTemplateCreateCommand(NotificationTemplateCreateRequest request)
        {
            Request = request;
        }
    }
}

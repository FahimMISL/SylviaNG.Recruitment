using MediatR;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Commands.NotificationTemplateDelete
{
    public class NotificationTemplateDeleteCommand : IRequest<Unit>
    {
        public long NotificationTemplateId { get; set; }

        public NotificationTemplateDeleteCommand(long notificationTemplateId)
        {
            NotificationTemplateId = notificationTemplateId;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Commands.NotificationTemplateDelete
{
    public class NotificationTemplateDeleteHandler : IRequestHandler<NotificationTemplateDeleteCommand, Unit>
    {
        private readonly INotificationTemplateService _notificationTemplateService;

        public NotificationTemplateDeleteHandler(INotificationTemplateService notificationTemplateService)
        {
            _notificationTemplateService = notificationTemplateService;
        }

        public async Task<Unit> Handle(NotificationTemplateDeleteCommand command, CancellationToken cancellationToken)
        {
            await _notificationTemplateService.DeleteAsync(command.NotificationTemplateId);
            return Unit.Value;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Commands.NotificationTemplateUpdate
{
    public class NotificationTemplateUpdateHandler : IRequestHandler<NotificationTemplateUpdateCommand, Unit>
    {
        private readonly INotificationTemplateService _notificationTemplateService;

        public NotificationTemplateUpdateHandler(INotificationTemplateService notificationTemplateService)
        {
            _notificationTemplateService = notificationTemplateService;
        }

        public async Task<Unit> Handle(NotificationTemplateUpdateCommand command, CancellationToken cancellationToken)
        {
            await _notificationTemplateService.UpdateAsync(command.NotificationTemplateId, command.Request);
            return Unit.Value;
        }
    }
}

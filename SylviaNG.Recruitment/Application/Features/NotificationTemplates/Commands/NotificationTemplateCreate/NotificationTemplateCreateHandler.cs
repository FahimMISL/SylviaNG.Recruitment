using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Commands.NotificationTemplateCreate
{
    public class NotificationTemplateCreateHandler : IRequestHandler<NotificationTemplateCreateCommand, long>
    {
        private readonly INotificationTemplateService _notificationTemplateService;

        public NotificationTemplateCreateHandler(INotificationTemplateService notificationTemplateService)
        {
            _notificationTemplateService = notificationTemplateService;
        }

        public async Task<long> Handle(NotificationTemplateCreateCommand command, CancellationToken cancellationToken)
        {
            return await _notificationTemplateService.CreateAsync(command.Request);
        }
    }
}

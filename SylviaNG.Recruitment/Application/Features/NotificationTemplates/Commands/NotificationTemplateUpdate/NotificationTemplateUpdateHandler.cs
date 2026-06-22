using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Commands.NotificationTemplateUpdate
{
    public class NotificationTemplateUpdateHandler : IRequestHandler<NotificationTemplateUpdateCommand, Unit>
    {
        private readonly INotificationTemplateService _service;

        public NotificationTemplateUpdateHandler(INotificationTemplateService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(NotificationTemplateUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.NotificationTemplateId, command.Request);
            return Unit.Value;
        }
    }
}

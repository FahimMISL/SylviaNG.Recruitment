using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Commands.NotificationTemplateDelete
{
    public class NotificationTemplateDeleteHandler : IRequestHandler<NotificationTemplateDeleteCommand, Unit>
    {
        private readonly INotificationTemplateService _service;

        public NotificationTemplateDeleteHandler(INotificationTemplateService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(NotificationTemplateDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.NotificationTemplateId);
            return Unit.Value;
        }
    }
}

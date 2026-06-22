using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Commands.NotificationTemplateCreate
{
    public class NotificationTemplateCreateHandler : IRequestHandler<NotificationTemplateCreateCommand, long>
    {
        private readonly INotificationTemplateService _service;

        public NotificationTemplateCreateHandler(INotificationTemplateService service)
        {
            _service = service;
        }

        public async Task<long> Handle(NotificationTemplateCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.NotificationLogCreate
{
    public class NotificationLogCreateHandler : IRequestHandler<NotificationLogCreateCommand, long>
    {
        private readonly INotificationLogService _service;

        public NotificationLogCreateHandler(INotificationLogService service)
        {
            _service = service;
        }

        public async Task<long> Handle(NotificationLogCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.NotificationLogUpdate
{
    public class NotificationLogUpdateHandler : IRequestHandler<NotificationLogUpdateCommand, Unit>
    {
        private readonly INotificationLogService _service;

        public NotificationLogUpdateHandler(INotificationLogService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(NotificationLogUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.NotificationLogId, command.Request);
            return Unit.Value;
        }
    }
}

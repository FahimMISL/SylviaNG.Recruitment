using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.NotificationLogDelete
{
    public class NotificationLogDeleteHandler : IRequestHandler<NotificationLogDeleteCommand, Unit>
    {
        private readonly INotificationLogService _service;

        public NotificationLogDeleteHandler(INotificationLogService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(NotificationLogDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.NotificationLogId);
            return Unit.Value;
        }
    }
}

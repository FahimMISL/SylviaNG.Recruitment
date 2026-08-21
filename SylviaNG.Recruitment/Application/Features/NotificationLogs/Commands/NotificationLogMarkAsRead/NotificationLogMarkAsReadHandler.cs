using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.NotificationLogMarkAsRead
{
    public class NotificationLogMarkAsReadHandler : IRequestHandler<NotificationLogMarkAsReadCommand, Unit>
    {
        private readonly INotificationLogService _notificationLogService;

        public NotificationLogMarkAsReadHandler(INotificationLogService notificationLogService)
        {
            _notificationLogService = notificationLogService;
        }

        public async Task<Unit> Handle(NotificationLogMarkAsReadCommand command, CancellationToken cancellationToken)
        {
            await _notificationLogService.MarkAsReadAsync(command.NotificationLogId);
            return Unit.Value;
        }
    }
}

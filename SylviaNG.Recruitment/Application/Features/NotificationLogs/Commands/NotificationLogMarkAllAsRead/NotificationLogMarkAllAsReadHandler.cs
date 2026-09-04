using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.NotificationLogMarkAllAsRead
{
    public class NotificationLogMarkAllAsReadHandler : IRequestHandler<NotificationLogMarkAllAsReadCommand, int>
    {
        private readonly INotificationLogService _notificationLogService;

        public NotificationLogMarkAllAsReadHandler(INotificationLogService notificationLogService)
        {
            _notificationLogService = notificationLogService;
        }

        public async Task<int> Handle(NotificationLogMarkAllAsReadCommand command, CancellationToken cancellationToken)
        {
            return await _notificationLogService.MarkAllAsReadAsync();
        }
    }
}

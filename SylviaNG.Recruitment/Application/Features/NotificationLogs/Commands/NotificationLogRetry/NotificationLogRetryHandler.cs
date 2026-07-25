using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.NotificationLogRetry
{
    public class NotificationLogRetryHandler : IRequestHandler<NotificationLogRetryCommand, NotificationLogResponse>
    {
        private readonly INotificationLogService _notificationLogService;

        public NotificationLogRetryHandler(INotificationLogService notificationLogService)
        {
            _notificationLogService = notificationLogService;
        }

        public async Task<NotificationLogResponse> Handle(NotificationLogRetryCommand command, CancellationToken cancellationToken)
        {
            return await _notificationLogService.RetryAsync(command.NotificationLogId);
        }
    }
}

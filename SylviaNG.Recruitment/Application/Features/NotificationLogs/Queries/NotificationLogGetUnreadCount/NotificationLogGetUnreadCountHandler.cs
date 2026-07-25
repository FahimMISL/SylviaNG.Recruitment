using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Queries.NotificationLogGetUnreadCount
{
    public class NotificationLogGetUnreadCountHandler : IRequestHandler<NotificationLogGetUnreadCountQuery, int>
    {
        private readonly INotificationLogService _notificationLogService;

        public NotificationLogGetUnreadCountHandler(INotificationLogService notificationLogService)
        {
            _notificationLogService = notificationLogService;
        }

        public async Task<int> Handle(NotificationLogGetUnreadCountQuery query, CancellationToken cancellationToken)
        {
            return await _notificationLogService.GetUnreadCountAsync();
        }
    }
}

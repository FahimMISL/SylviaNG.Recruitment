using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Queries.NotificationLogGetUnread
{
    public class NotificationLogGetUnreadHandler : IRequestHandler<NotificationLogGetUnreadQuery, List<NotificationLogResponse>>
    {
        private readonly INotificationLogService _notificationLogService;

        public NotificationLogGetUnreadHandler(INotificationLogService notificationLogService)
        {
            _notificationLogService = notificationLogService;
        }

        public async Task<List<NotificationLogResponse>> Handle(NotificationLogGetUnreadQuery query, CancellationToken cancellationToken)
        {
            return await _notificationLogService.GetUnreadAsync();
        }
    }
}

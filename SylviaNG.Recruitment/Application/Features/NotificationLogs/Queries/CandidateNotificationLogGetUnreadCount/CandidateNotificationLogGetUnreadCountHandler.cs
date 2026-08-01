using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Queries.CandidateNotificationLogGetUnreadCount
{
    public class CandidateNotificationLogGetUnreadCountHandler : IRequestHandler<CandidateNotificationLogGetUnreadCountQuery, int>
    {
        private readonly INotificationLogService _notificationLogService;

        public CandidateNotificationLogGetUnreadCountHandler(INotificationLogService notificationLogService)
        {
            _notificationLogService = notificationLogService;
        }

        public async Task<int> Handle(CandidateNotificationLogGetUnreadCountQuery query, CancellationToken cancellationToken)
        {
            return await _notificationLogService.GetUnreadCountForCurrentCandidateAsync();
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Queries.CandidateNotificationLogGetUnread
{
    public class CandidateNotificationLogGetUnreadHandler : IRequestHandler<CandidateNotificationLogGetUnreadQuery, List<NotificationLogResponse>>
    {
        private readonly INotificationLogService _notificationLogService;

        public CandidateNotificationLogGetUnreadHandler(INotificationLogService notificationLogService)
        {
            _notificationLogService = notificationLogService;
        }

        public async Task<List<NotificationLogResponse>> Handle(CandidateNotificationLogGetUnreadQuery query, CancellationToken cancellationToken)
        {
            return await _notificationLogService.GetUnreadForCurrentCandidateAsync();
        }
    }
}

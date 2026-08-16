using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.CandidateNotificationLogMarkAllAsRead
{
    public class CandidateNotificationLogMarkAllAsReadHandler : IRequestHandler<CandidateNotificationLogMarkAllAsReadCommand, int>
    {
        private readonly INotificationLogService _notificationLogService;

        public CandidateNotificationLogMarkAllAsReadHandler(INotificationLogService notificationLogService)
        {
            _notificationLogService = notificationLogService;
        }

        public async Task<int> Handle(CandidateNotificationLogMarkAllAsReadCommand command, CancellationToken cancellationToken)
        {
            return await _notificationLogService.MarkAllAsReadForCurrentCandidateAsync();
        }
    }
}

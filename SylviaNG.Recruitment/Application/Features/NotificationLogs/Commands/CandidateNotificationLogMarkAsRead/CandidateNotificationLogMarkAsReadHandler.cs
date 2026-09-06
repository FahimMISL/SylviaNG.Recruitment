using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.CandidateNotificationLogMarkAsRead
{
    public class CandidateNotificationLogMarkAsReadHandler : IRequestHandler<CandidateNotificationLogMarkAsReadCommand, Unit>
    {
        private readonly INotificationLogService _notificationLogService;

        public CandidateNotificationLogMarkAsReadHandler(INotificationLogService notificationLogService)
        {
            _notificationLogService = notificationLogService;
        }

        public async Task<Unit> Handle(CandidateNotificationLogMarkAsReadCommand command, CancellationToken cancellationToken)
        {
            await _notificationLogService.MarkAsReadForCurrentCandidateAsync(command.NotificationLogId);
            return Unit.Value;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Queries.NotificationLogGetAll
{
    public class NotificationLogGetAllHandler : IRequestHandler<NotificationLogGetAllQuery, PagedResult<NotificationLogResponse>>
    {
        private readonly INotificationLogService _notificationLogService;

        public NotificationLogGetAllHandler(INotificationLogService notificationLogService)
        {
            _notificationLogService = notificationLogService;
        }

        public async Task<PagedResult<NotificationLogResponse>> Handle(NotificationLogGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _notificationLogService.GetFilteredAsync(query.Filter);
        }
    }
}

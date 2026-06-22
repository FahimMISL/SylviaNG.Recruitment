using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Queries.NotificationLogGetAllPaged
{
    public class NotificationLogGetAllPagedHandler : IRequestHandler<NotificationLogGetAllPagedQuery, PagedResult<NotificationLogResponse>>
    {
        private readonly INotificationLogService _notificationLogService;

        public NotificationLogGetAllPagedHandler(INotificationLogService notificationLogService)
        {
            _notificationLogService = notificationLogService;
        }

        public async Task<PagedResult<NotificationLogResponse>> Handle(NotificationLogGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _notificationLogService.GetPaginatedAsync(query.Request);
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationEvents.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.NotificationEvents.Queries.NotificationEventGetAllPaged
{
    public class NotificationEventGetAllPagedHandler : IRequestHandler<NotificationEventGetAllPagedQuery, PagedResult<NotificationEventResponse>>
    {
        private readonly INotificationEventService _notificationEventService;

        public NotificationEventGetAllPagedHandler(INotificationEventService notificationEventService)
        {
            _notificationEventService = notificationEventService;
        }

        public async Task<PagedResult<NotificationEventResponse>> Handle(NotificationEventGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _notificationEventService.GetPaginatedAsync(query.Request);
        }
    }
}

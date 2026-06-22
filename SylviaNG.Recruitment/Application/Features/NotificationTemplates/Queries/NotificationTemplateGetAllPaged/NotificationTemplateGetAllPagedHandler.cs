using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Queries.NotificationTemplateGetAllPaged
{
    public class NotificationTemplateGetAllPagedHandler : IRequestHandler<NotificationTemplateGetAllPagedQuery, PagedResult<NotificationTemplateResponse>>
    {
        private readonly INotificationTemplateService _notificationTemplateService;

        public NotificationTemplateGetAllPagedHandler(INotificationTemplateService notificationTemplateService)
        {
            _notificationTemplateService = notificationTemplateService;
        }

        public async Task<PagedResult<NotificationTemplateResponse>> Handle(NotificationTemplateGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _notificationTemplateService.GetPaginatedAsync(query.Request);
        }
    }
}

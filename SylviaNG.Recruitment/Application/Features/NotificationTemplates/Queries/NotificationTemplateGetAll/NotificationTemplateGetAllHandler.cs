using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Queries.NotificationTemplateGetAll
{
    public class NotificationTemplateGetAllHandler : IRequestHandler<NotificationTemplateGetAllQuery, List<NotificationTemplateResponse>>
    {
        private readonly INotificationTemplateService _notificationTemplateService;

        public NotificationTemplateGetAllHandler(INotificationTemplateService notificationTemplateService)
        {
            _notificationTemplateService = notificationTemplateService;
        }

        public async Task<List<NotificationTemplateResponse>> Handle(NotificationTemplateGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _notificationTemplateService.GetAllAsync();
        }
    }
}

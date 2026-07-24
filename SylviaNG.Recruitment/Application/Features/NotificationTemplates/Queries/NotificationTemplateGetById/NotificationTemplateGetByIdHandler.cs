using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Queries.NotificationTemplateGetById
{
    public class NotificationTemplateGetByIdHandler : IRequestHandler<NotificationTemplateGetByIdQuery, NotificationTemplateResponse>
    {
        private readonly INotificationTemplateService _notificationTemplateService;

        public NotificationTemplateGetByIdHandler(INotificationTemplateService notificationTemplateService)
        {
            _notificationTemplateService = notificationTemplateService;
        }

        public async Task<NotificationTemplateResponse> Handle(NotificationTemplateGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _notificationTemplateService.GetByIdAsync(query.NotificationTemplateId);
        }
    }
}

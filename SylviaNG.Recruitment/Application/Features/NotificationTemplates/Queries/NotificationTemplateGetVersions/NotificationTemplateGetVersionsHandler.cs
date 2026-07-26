using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Queries.NotificationTemplateGetVersions
{
    public class NotificationTemplateGetVersionsHandler : IRequestHandler<NotificationTemplateGetVersionsQuery, List<NotificationTemplateVersionResponse>>
    {
        private readonly INotificationTemplateService _notificationTemplateService;

        public NotificationTemplateGetVersionsHandler(INotificationTemplateService notificationTemplateService)
        {
            _notificationTemplateService = notificationTemplateService;
        }

        public async Task<List<NotificationTemplateVersionResponse>> Handle(NotificationTemplateGetVersionsQuery query, CancellationToken cancellationToken)
        {
            return await _notificationTemplateService.GetVersionsAsync(query.NotificationTemplateId);
        }
    }
}

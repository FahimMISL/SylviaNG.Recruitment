using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Queries.NotificationTemplateGetById
{
    public class NotificationTemplateGetByIdHandler : IRequestHandler<NotificationTemplateGetByIdQuery, NotificationTemplateResponse>
    {
        private readonly INotificationTemplateService _service;

        public NotificationTemplateGetByIdHandler(INotificationTemplateService service)
        {
            _service = service;
        }

        public async Task<NotificationTemplateResponse> Handle(NotificationTemplateGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.NotificationTemplateId);
        }
    }
}

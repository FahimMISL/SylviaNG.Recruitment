using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Queries.NotificationLogGetById
{
    public class NotificationLogGetByIdHandler : IRequestHandler<NotificationLogGetByIdQuery, NotificationLogResponse>
    {
        private readonly INotificationLogService _service;

        public NotificationLogGetByIdHandler(INotificationLogService service)
        {
            _service = service;
        }

        public async Task<NotificationLogResponse> Handle(NotificationLogGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.NotificationLogId);
        }
    }
}

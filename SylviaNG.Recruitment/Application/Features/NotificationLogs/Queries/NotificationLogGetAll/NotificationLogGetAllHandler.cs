using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Queries.NotificationLogGetAll
{
    public class NotificationLogGetAllHandler : IRequestHandler<NotificationLogGetAllQuery, List<NotificationLogResponse>>
    {
        private readonly INotificationLogService _service;

        public NotificationLogGetAllHandler(INotificationLogService service)
        {
            _service = service;
        }

        public async Task<List<NotificationLogResponse>> Handle(NotificationLogGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}

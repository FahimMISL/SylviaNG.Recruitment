using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationEvents.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationEvents.Queries.NotificationEventGetAll
{
    public class NotificationEventGetAllHandler : IRequestHandler<NotificationEventGetAllQuery, List<NotificationEventResponse>>
    {
        private readonly INotificationEventService _service;

        public NotificationEventGetAllHandler(INotificationEventService service)
        {
            _service = service;
        }

        public async Task<List<NotificationEventResponse>> Handle(NotificationEventGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}

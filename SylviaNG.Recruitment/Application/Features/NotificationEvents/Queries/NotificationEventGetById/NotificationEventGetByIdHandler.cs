using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationEvents.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationEvents.Queries.NotificationEventGetById
{
    public class NotificationEventGetByIdHandler : IRequestHandler<NotificationEventGetByIdQuery, NotificationEventResponse>
    {
        private readonly INotificationEventService _service;

        public NotificationEventGetByIdHandler(INotificationEventService service)
        {
            _service = service;
        }

        public async Task<NotificationEventResponse> Handle(NotificationEventGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.NotificationEventId);
        }
    }
}

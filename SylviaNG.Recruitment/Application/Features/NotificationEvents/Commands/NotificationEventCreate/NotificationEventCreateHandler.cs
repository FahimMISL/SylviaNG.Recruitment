using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationEvents.Commands.NotificationEventCreate
{
    public class NotificationEventCreateHandler : IRequestHandler<NotificationEventCreateCommand, long>
    {
        private readonly INotificationEventService _service;

        public NotificationEventCreateHandler(INotificationEventService service)
        {
            _service = service;
        }

        public async Task<long> Handle(NotificationEventCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationEvents.Commands.NotificationEventUpdate
{
    public class NotificationEventUpdateHandler : IRequestHandler<NotificationEventUpdateCommand, Unit>
    {
        private readonly INotificationEventService _service;

        public NotificationEventUpdateHandler(INotificationEventService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(NotificationEventUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.NotificationEventId, command.Request);
            return Unit.Value;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationEvents.Commands.NotificationEventDelete
{
    public class NotificationEventDeleteHandler : IRequestHandler<NotificationEventDeleteCommand, Unit>
    {
        private readonly INotificationEventService _service;

        public NotificationEventDeleteHandler(INotificationEventService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(NotificationEventDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.NotificationEventId);
            return Unit.Value;
        }
    }
}

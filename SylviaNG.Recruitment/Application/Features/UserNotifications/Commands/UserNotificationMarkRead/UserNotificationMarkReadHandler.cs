using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.UserNotifications.Commands.UserNotificationMarkRead;

public class UserNotificationMarkReadHandler : IRequestHandler<UserNotificationMarkReadCommand>
{
    private readonly IUserNotificationService _service;

    public UserNotificationMarkReadHandler(IUserNotificationService service)
    {
        _service = service;
    }

    public async Task Handle(UserNotificationMarkReadCommand command, CancellationToken cancellationToken)
    {
        await _service.MarkAsReadAsync(command.NotificationId, command.KeycloakUserId);
    }
}

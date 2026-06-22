using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.UserNotifications.Commands.UserNotificationMarkAllRead;

public class UserNotificationMarkAllReadHandler : IRequestHandler<UserNotificationMarkAllReadCommand>
{
    private readonly IUserNotificationService _service;

    public UserNotificationMarkAllReadHandler(IUserNotificationService service)
    {
        _service = service;
    }

    public async Task Handle(UserNotificationMarkAllReadCommand command, CancellationToken cancellationToken)
    {
        await _service.MarkAllAsReadAsync(command.KeycloakUserId);
    }
}

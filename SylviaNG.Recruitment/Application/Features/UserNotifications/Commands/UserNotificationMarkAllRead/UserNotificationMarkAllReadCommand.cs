using MediatR;

namespace SylviaNG.Recruitment.Application.Features.UserNotifications.Commands.UserNotificationMarkAllRead;

public class UserNotificationMarkAllReadCommand : IRequest
{
    public string KeycloakUserId { get; set; }

    public UserNotificationMarkAllReadCommand(string keycloakUserId)
    {
        KeycloakUserId = keycloakUserId;
    }
}

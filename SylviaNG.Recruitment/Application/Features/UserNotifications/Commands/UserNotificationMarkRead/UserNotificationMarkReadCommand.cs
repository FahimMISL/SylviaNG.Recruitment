using MediatR;

namespace SylviaNG.Recruitment.Application.Features.UserNotifications.Commands.UserNotificationMarkRead;

public class UserNotificationMarkReadCommand : IRequest
{
    public long NotificationId { get; set; }
    public string KeycloakUserId { get; set; }

    public UserNotificationMarkReadCommand(long notificationId, string keycloakUserId)
    {
        NotificationId = notificationId;
        KeycloakUserId = keycloakUserId;
    }
}

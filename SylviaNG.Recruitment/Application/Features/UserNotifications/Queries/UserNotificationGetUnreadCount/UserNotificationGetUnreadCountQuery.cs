using MediatR;

namespace SylviaNG.Recruitment.Application.Features.UserNotifications.Queries.UserNotificationGetUnreadCount;

public class UserNotificationGetUnreadCountQuery : IRequest<int>
{
    public string KeycloakUserId { get; set; }

    public UserNotificationGetUnreadCountQuery(string keycloakUserId)
    {
        KeycloakUserId = keycloakUserId;
    }
}

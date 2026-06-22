using MediatR;
using SylviaNG.Recruitment.Application.Features.UserNotifications.Models;

namespace SylviaNG.Recruitment.Application.Features.UserNotifications.Queries.UserNotificationGetByUser;

public class UserNotificationGetByUserQuery : IRequest<List<UserNotificationResponse>>
{
    public string KeycloakUserId { get; set; }
    public int Limit { get; set; }

    public UserNotificationGetByUserQuery(string keycloakUserId, int limit = 20)
    {
        KeycloakUserId = keycloakUserId;
        Limit = limit;
    }
}

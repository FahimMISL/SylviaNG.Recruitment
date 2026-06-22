using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.UserNotifications.Queries.UserNotificationGetUnreadCount;

public class UserNotificationGetUnreadCountHandler : IRequestHandler<UserNotificationGetUnreadCountQuery, int>
{
    private readonly IUserNotificationService _service;

    public UserNotificationGetUnreadCountHandler(IUserNotificationService service)
    {
        _service = service;
    }

    public async Task<int> Handle(UserNotificationGetUnreadCountQuery query, CancellationToken cancellationToken)
    {
        return await _service.GetUnreadCountAsync(query.KeycloakUserId);
    }
}

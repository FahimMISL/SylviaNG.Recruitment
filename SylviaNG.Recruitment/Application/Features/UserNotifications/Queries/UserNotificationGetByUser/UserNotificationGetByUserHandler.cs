using MediatR;
using SylviaNG.Recruitment.Application.Features.UserNotifications.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.UserNotifications.Queries.UserNotificationGetByUser;

public class UserNotificationGetByUserHandler : IRequestHandler<UserNotificationGetByUserQuery, List<UserNotificationResponse>>
{
    private readonly IUserNotificationService _service;

    public UserNotificationGetByUserHandler(IUserNotificationService service)
    {
        _service = service;
    }

    public async Task<List<UserNotificationResponse>> Handle(UserNotificationGetByUserQuery query, CancellationToken cancellationToken)
    {
        return await _service.GetByUserAsync(query.KeycloakUserId, query.Limit);
    }
}

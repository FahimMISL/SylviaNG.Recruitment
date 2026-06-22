using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.UserNotifications.Commands.UserNotificationCreate;

public class UserNotificationCreateHandler : IRequestHandler<UserNotificationCreateCommand, long>
{
    private readonly IUserNotificationService _service;

    public UserNotificationCreateHandler(IUserNotificationService service)
    {
        _service = service;
    }

    public async Task<long> Handle(UserNotificationCreateCommand command, CancellationToken cancellationToken)
    {
        return await _service.CreateAsync(command.Request);
    }
}

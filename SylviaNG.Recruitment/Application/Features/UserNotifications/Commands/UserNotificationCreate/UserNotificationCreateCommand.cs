using MediatR;
using SylviaNG.Recruitment.Application.Features.UserNotifications.Models;

namespace SylviaNG.Recruitment.Application.Features.UserNotifications.Commands.UserNotificationCreate;

public class UserNotificationCreateCommand : IRequest<long>
{
    public UserNotificationCreateRequest Request { get; set; }

    public UserNotificationCreateCommand(UserNotificationCreateRequest request)
    {
        Request = request;
    }
}

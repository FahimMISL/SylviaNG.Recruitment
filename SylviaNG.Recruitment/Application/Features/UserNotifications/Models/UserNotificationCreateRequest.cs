using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.UserNotifications.Models;

public class UserNotificationCreateRequest
{
    public string KeycloakUserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public UserNotificationTypeEnum NotificationType { get; set; } = UserNotificationTypeEnum.Info;
    public string? ActionUrl { get; set; }
}

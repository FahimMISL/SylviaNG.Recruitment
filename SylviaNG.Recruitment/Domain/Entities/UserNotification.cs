using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class UserNotification : Audit
{
    public long UserNotificationId { get; set; }
    public string KeycloakUserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public UserNotificationTypeEnum NotificationType { get; set; } = UserNotificationTypeEnum.Info;
    public string? ActionUrl { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public bool IsActive { get; set; } = true;
}

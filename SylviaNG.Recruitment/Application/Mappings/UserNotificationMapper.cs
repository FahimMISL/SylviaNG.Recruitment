using SylviaNG.Recruitment.Application.Features.UserNotifications.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings;

public static class UserNotificationMapper
{
    public static UserNotification ToEntity(this UserNotificationCreateRequest request)
    {
        return new UserNotification
        {
            KeycloakUserId = request.KeycloakUserId,
            Title = request.Title,
            Message = request.Message,
            NotificationType = request.NotificationType,
            ActionUrl = request.ActionUrl,
            IsRead = false,
            IsActive = true,
        };
    }

    public static UserNotificationResponse ToResponse(this UserNotification entity)
    {
        return new UserNotificationResponse
        {
            UserNotificationId = entity.UserNotificationId,
            Title = entity.Title,
            Message = entity.Message,
            NotificationType = entity.NotificationType,
            ActionUrl = entity.ActionUrl,
            IsRead = entity.IsRead,
            CreatedAt = entity.CreatedAt,
            ReadAt = entity.ReadAt,
        };
    }
}

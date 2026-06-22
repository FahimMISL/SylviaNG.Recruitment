using SylviaNG.Recruitment.Application.Features.UserNotifications.Models;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Interfaces.Services;

public interface IUserNotificationService
{
    Task<List<UserNotificationResponse>> GetByUserAsync(string keycloakUserId, int limit = 20);
    Task<int> GetUnreadCountAsync(string keycloakUserId);
    Task<long> CreateAsync(UserNotificationCreateRequest request);
    Task MarkAsReadAsync(long notificationId, string keycloakUserId);
    Task MarkAllAsReadAsync(string keycloakUserId);
    Task ClearAllAsync(string keycloakUserId);
    Task NotifyRoleAsync(string roleName, string title, string message, UserNotificationTypeEnum type = UserNotificationTypeEnum.Info, string? actionUrl = null);
    Task NotifyUserAsync(string keycloakUserId, string title, string message, UserNotificationTypeEnum type = UserNotificationTypeEnum.Info, string? actionUrl = null);
}

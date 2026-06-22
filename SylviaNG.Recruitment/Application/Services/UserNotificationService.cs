using SylviaNG.Recruitment.Application.Features.UserNotifications.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services;

public class UserNotificationService : IUserNotificationService
{
    private readonly IUserNotificationRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IKeycloakAdminService _keycloak;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserNotificationService> _logger;

    public UserNotificationService(IUserNotificationRepository repository, IUserRepository userRepository, IKeycloakAdminService keycloak, IUnitOfWork unitOfWork, ILogger<UserNotificationService> logger)
    {
        _repository = repository;
        _userRepository = userRepository;
        _keycloak = keycloak;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<List<UserNotificationResponse>> GetByUserAsync(string keycloakUserId, int limit = 20)
    {
        var entities = await _repository.GetByKeycloakUserIdAsync(keycloakUserId, limit);
        return entities.Select(e => e.ToResponse()).ToList();
    }

    public async Task<int> GetUnreadCountAsync(string keycloakUserId)
    {
        return await _repository.GetUnreadCountAsync(keycloakUserId);
    }

    public async Task<long> CreateAsync(UserNotificationCreateRequest request)
    {
        var entity = request.ToEntity();
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.UserNotificationId;
    }

    public async Task MarkAsReadAsync(long notificationId, string keycloakUserId)
    {
        var entity = await _repository.GetByIdAsync(notificationId)
            ?? throw new KeyNotFoundException($"Notification with ID {notificationId} not found.");

        if (entity.KeycloakUserId != keycloakUserId)
            throw new UnauthorizedAccessException("Cannot mark another user's notification as read.");

        entity.IsRead = true;
        entity.ReadAt = DateTime.UtcNow;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task MarkAllAsReadAsync(string keycloakUserId)
    {
        var unread = await _repository.GetByKeycloakUserIdAsync(keycloakUserId, 100);
        var toMark = unread.Where(n => !n.IsRead).ToList();

        foreach (var n in toMark)
        {
            n.IsRead = true;
            n.ReadAt = DateTime.UtcNow;
            _repository.Update(n);
        }

        if (toMark.Count > 0)
            await _unitOfWork.SaveChangesAsync();
    }

    public async Task ClearAllAsync(string keycloakUserId)
    {
        var all = await _repository.GetByKeycloakUserIdAsync(keycloakUserId, 100);
        foreach (var n in all)
        {
            n.IsActive = false;
            _repository.Update(n);
        }
        if (all.Count > 0)
            await _unitOfWork.SaveChangesAsync();
    }

    public async Task NotifyRoleAsync(string roleName, string title, string message, UserNotificationTypeEnum type = UserNotificationTypeEnum.Info, string? actionUrl = null)
    {
        _logger.LogInformation("NotifyRoleAsync called: role={Role}, title={Title}", roleName, title);
        var keycloakUserIds = await _keycloak.GetUserIdsByRoleAsync(roleName);
        _logger.LogInformation("Keycloak returned {Count} users for role {Role}: [{Ids}]", keycloakUserIds.Count, roleName, string.Join(", ", keycloakUserIds));
        if (keycloakUserIds.Count == 0) return;

        foreach (var kcUserId in keycloakUserIds)
        {
            var entity = new UserNotificationCreateRequest
            {
                KeycloakUserId = kcUserId,
                Title = title,
                Message = message,
                NotificationType = type,
                ActionUrl = actionUrl,
            }.ToEntity();
            await _repository.AddAsync(entity);
        }
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task NotifyUserAsync(string keycloakUserId, string title, string message, UserNotificationTypeEnum type = UserNotificationTypeEnum.Info, string? actionUrl = null)
    {
        var entity = new UserNotificationCreateRequest
        {
            KeycloakUserId = keycloakUserId,
            Title = title,
            Message = message,
            NotificationType = type,
            ActionUrl = actionUrl,
        }.ToEntity();
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
    }
}

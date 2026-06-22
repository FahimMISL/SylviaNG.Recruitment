using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories;

public interface IUserNotificationRepository : IRepository<UserNotification>
{
    Task<List<UserNotification>> GetByKeycloakUserIdAsync(string keycloakUserId, int limit = 20);
    Task<int> GetUnreadCountAsync(string keycloakUserId);
}

using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories;

public class UserNotificationRepository : Repository<UserNotification>, IUserNotificationRepository
{
    private readonly ApplicationDBContext _context;

    public UserNotificationRepository(ApplicationDBContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<UserNotification>> GetByKeycloakUserIdAsync(string keycloakUserId, int limit = 20)
    {
        return await _context.Set<UserNotification>()
            .Where(n => n.KeycloakUserId == keycloakUserId && n.IsActive)
            .OrderByDescending(n => n.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(string keycloakUserId)
    {
        return await _context.Set<UserNotification>()
            .CountAsync(n => n.KeycloakUserId == keycloakUserId && !n.IsRead && n.IsActive);
    }
}

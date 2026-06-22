using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories;

public class UserProfilePhotoRepository : Repository<UserProfilePhoto>, IUserProfilePhotoRepository
{
    private readonly ApplicationDBContext _context;

    public UserProfilePhotoRepository(ApplicationDBContext context) : base(context)
    {
        _context = context;
    }

    public async Task<UserProfilePhoto?> GetByKeycloakUserIdAsync(string keycloakUserId)
    {
        return await _context.Set<UserProfilePhoto>()
            .FirstOrDefaultAsync(p => p.KeycloakUserId == keycloakUserId && p.IsActive);
    }
}

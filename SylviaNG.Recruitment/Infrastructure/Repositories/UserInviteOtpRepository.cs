using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class UserInviteOtpRepository : Repository<UserInviteOtp>, IUserInviteOtpRepository
    {
        public UserInviteOtpRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<UserInviteOtp?> GetByChallengeIdAsync(Guid challengeId)
        {
            return await _dbSet.FirstOrDefaultAsync(o => o.ChallengeId == challengeId);
        }
    }
}

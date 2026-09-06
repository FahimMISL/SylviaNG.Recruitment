using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class PasswordResetOtpRepository : Repository<PasswordResetOtp>, IPasswordResetOtpRepository
    {
        public PasswordResetOtpRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<PasswordResetOtp?> GetByChallengeIdAsync(Guid challengeId)
        {
            return await _dbSet.FirstOrDefaultAsync(o => o.ChallengeId == challengeId);
        }
    }
}

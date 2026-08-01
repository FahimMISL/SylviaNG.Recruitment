using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class CandidateLoginOtpRepository : Repository<CandidateLoginOtp>, ICandidateLoginOtpRepository
    {
        public CandidateLoginOtpRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<CandidateLoginOtp?> GetByChallengeIdAsync(Guid challengeId)
        {
            return await _dbSet.FirstOrDefaultAsync(o => o.ChallengeId == challengeId);
        }

        public async Task<bool> HasEverVerifiedAsync(string username)
        {
            return await _dbSet.AnyAsync(o => o.Username == username && o.ConsumedAtUtc != null);
        }
    }
}

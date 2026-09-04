using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class ImpersonationSessionRepository : Repository<ImpersonationSession>, IImpersonationSessionRepository
    {
        public ImpersonationSessionRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<ImpersonationSession?> GetActiveByActorUserAccountIdAsync(long actorUserAccountId, DateTime nowUtc)
        {
            return await _dbSet
                .Include(s => s.TargetUserAccount)
                .Where(s => s.ActorUserAccountId == actorUserAccountId && s.EndedAt == null && s.ExpiresAt > nowUtc)
                .OrderByDescending(s => s.StartedAt)
                .FirstOrDefaultAsync();
        }
    }
}

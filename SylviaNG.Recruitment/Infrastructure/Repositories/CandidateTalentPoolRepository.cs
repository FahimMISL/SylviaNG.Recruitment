using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class CandidateTalentPoolRepository : Repository<CandidateTalentPool>, ICandidateTalentPoolRepository
    {
        public CandidateTalentPoolRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<HashSet<long>> GetExistingCandidateProfileIdsAsync(IEnumerable<long> candidateProfileIds)
        {
            var ids = candidateProfileIds.ToList();
            var existing = await _dbSet
                .Where(t => ids.Contains(t.CandidateProfileId))
                .Select(t => t.CandidateProfileId)
                .ToListAsync();

            return existing.ToHashSet();
        }

        public async Task<List<CandidateTalentPool>> GetAllWithProfileAsync()
        {
            return await _dbSet
                .Include(t => t.CandidateProfile)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<CandidateTalentPool?> GetByCandidateProfileIdAsync(long candidateProfileId)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.CandidateProfileId == candidateProfileId);
        }
    }
}

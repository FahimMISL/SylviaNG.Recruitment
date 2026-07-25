using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class TalentPoolCandidateRepository : Repository<TalentPoolCandidate>, ITalentPoolCandidateRepository
    {
        public TalentPoolCandidateRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<TalentPoolCandidate?> GetByPoolAndCandidateAsync(long talentPoolId, long candidateProfileId)
        {
            return await _dbSet.FirstOrDefaultAsync(
                t => t.TalentPoolId == talentPoolId && t.CandidateProfileId == candidateProfileId);
        }

        public async Task<List<long>> GetExistingCandidateIdsAsync(long talentPoolId, IEnumerable<long> candidateProfileIds)
        {
            var ids = candidateProfileIds.ToList();
            return await _dbSet
                .Where(t => t.TalentPoolId == talentPoolId && ids.Contains(t.CandidateProfileId))
                .Select(t => t.CandidateProfileId)
                .ToListAsync();
        }

        public async Task<List<TalentPoolCandidate>> GetAllByCandidateProfileIdAsync(long candidateProfileId)
        {
            return await _dbSet
                .Include(t => t.TalentPool)
                .Where(t => t.CandidateProfileId == candidateProfileId)
                .ToListAsync();
        }
    }
}

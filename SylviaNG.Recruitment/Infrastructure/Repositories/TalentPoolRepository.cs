using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class TalentPoolRepository : Repository<TalentPool>, ITalentPoolRepository
    {
        public TalentPoolRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(t => t.Name == name && (!excludeId.HasValue || t.TalentPoolId != excludeId.Value));
        }

        public async Task<List<TalentPool>> GetAllWithCandidateCountAsync(long? jobPostingId = null)
        {
            var query = _dbSet
                .Include(p => p.Candidates)
                .Include(p => p.JobPosting)
                .AsQueryable();

            if (jobPostingId.HasValue)
                query = query.Where(p => p.JobPostingId == jobPostingId.Value);

            return await query
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<TalentPool?> GetByIdWithCandidatesAsync(long talentPoolId)
        {
            return await _dbSet
                .Include(p => p.JobPosting)
                .Include(p => p.Candidates).ThenInclude(c => c.CandidateProfile).ThenInclude(cp => cp.Educations)
                .Include(p => p.Candidates).ThenInclude(c => c.CandidateProfile).ThenInclude(cp => cp.WorkExperiences)
                .Include(p => p.Candidates).ThenInclude(c => c.CandidateProfile).ThenInclude(cp => cp.Skills)
                .Include(p => p.Candidates).ThenInclude(c => c.CandidateProfile).ThenInclude(cp => cp.Certifications)
                .Include(p => p.Candidates).ThenInclude(c => c.CandidateProfile).ThenInclude(cp => cp.Documents)
                .FirstOrDefaultAsync(p => p.TalentPoolId == talentPoolId);
        }
    }
}

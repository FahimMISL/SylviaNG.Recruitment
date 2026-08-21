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

        public async Task<List<TalentPool>> GetAllForLookupAsync()
        {
            return await _dbSet
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<TalentPool?> GetByIdWithCandidatesAsync(long talentPoolId)
        {
            // AsSplitQuery: six sibling collections hang off Candidates.CandidateProfile in one
            // query, which without it becomes a single-query cartesian join - a pool of 100
            // candidates with a handful of rows in each of the six collections returns hundreds of
            // thousands of duplicated rows to build 100 objects. Split into one query per
            // collection instead; safe here since the whole graph is scoped to one TalentPoolId.
            return await _dbSet
                .AsSplitQuery()
                .Include(p => p.JobPosting)
                .Include(p => p.Candidates).ThenInclude(c => c.CandidateProfile).ThenInclude(cp => cp.Educations)
                .Include(p => p.Candidates).ThenInclude(c => c.CandidateProfile).ThenInclude(cp => cp.WorkExperiences)
                .Include(p => p.Candidates).ThenInclude(c => c.CandidateProfile).ThenInclude(cp => cp.Skills)
                .Include(p => p.Candidates).ThenInclude(c => c.CandidateProfile).ThenInclude(cp => cp.Certifications)
                .Include(p => p.Candidates).ThenInclude(c => c.CandidateProfile).ThenInclude(cp => cp.Documents)
                .Include(p => p.Candidates).ThenInclude(c => c.CandidateProfile).ThenInclude(cp => cp.Country)
                .FirstOrDefaultAsync(p => p.TalentPoolId == talentPoolId);
        }
    }
}

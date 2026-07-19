using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class CandidateProfileRepository : Repository<CandidateProfile>, ICandidateProfileRepository
    {
        public CandidateProfileRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<CandidateProfile?> GetByKeycloakSubjectIdAsync(string keycloakSubjectId)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.KeycloakSubjectId == keycloakSubjectId);
        }

        public async Task<PagedResult<CandidateProfile>> GetPagedAsync(PagedRequest request, List<long>? talentPoolIds = null, List<string>? tags = null)
        {
            var query = _dbSet.Where(c => c.IsActive).AsQueryable();

            if (talentPoolIds != null && talentPoolIds.Count > 0)
            {
                var matchingProfileIds = _dbContext.TalentPoolCandidates
                    .Where(tc => talentPoolIds.Contains(tc.TalentPoolId))
                    .Select(tc => tc.CandidateProfileId);

                query = query.Where(c => matchingProfileIds.Contains(c.CandidateProfileId));
            }

            if (tags != null && tags.Count > 0)
                query = query.Where(c => c.Tags.Any(t => tags.Contains(t.TagName)));

            return await query.ToPaginatedResultAsync(request);
        }

        public async Task<List<CandidateProfile>> GetByEmailsAsync(IEnumerable<string> emails)
        {
            var emailSet = emails.ToList();
            return await _dbSet
                .Include(c => c.Educations)
                .Include(c => c.WorkExperiences)
                .Include(c => c.Skills)
                .Include(c => c.Tags)
                .Where(c => emailSet.Contains(c.Email))
                .ToListAsync();
        }

        public async Task<List<CandidateProfile>> GetAllActiveWithDetailsAsync()
        {
            return await _dbSet
                .Include(c => c.Educations)
                .Include(c => c.WorkExperiences)
                .Include(c => c.Skills)
                .Include(c => c.Certifications)
                .Include(c => c.Documents)
                .Where(c => c.IsActive)
                .ToListAsync();
        }
    }
}

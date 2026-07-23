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

        public async Task<long?> GetIdByEmailAsync(string email)
        {
            return await _dbSet
                .Where(c => c.Email.ToLower() == email.ToLower())
                .Select(c => (long?)c.CandidateProfileId)
                .FirstOrDefaultAsync();
        }

        public async Task<PagedResult<CandidateProfile>> GetPagedAsync(PagedRequest request, List<long>? talentPoolIds = null, List<string>? tags = null)
        {
            // CalculateCompleteness (CandidateProfileMapper.ToSummaryResponse) reads these
            // collections - without the Includes they're always empty here, undercounting
            // every candidate's completeness in the list regardless of their real data.
            var query = _dbSet
                .Where(c => c.IsActive)
                .Include(c => c.Educations).ThenInclude(e => e.Degree)
                .Include(c => c.Educations).ThenInclude(e => e.MajorSubjectSscHsc)
                .Include(c => c.Educations).ThenInclude(e => e.MajorSubjectUniversity)
                .Include(c => c.WorkExperiences)
                .Include(c => c.Skills)
                .Include(c => c.Certifications)
                .Include(c => c.Documents)
                .Include(c => c.Gender)
                .AsSplitQuery()
                .AsQueryable();

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
                .Include(c => c.Educations).ThenInclude(e => e.Degree)
                .Include(c => c.Educations).ThenInclude(e => e.MajorSubjectSscHsc)
                .Include(c => c.Educations).ThenInclude(e => e.MajorSubjectUniversity)
                .Include(c => c.WorkExperiences)
                .Include(c => c.Skills)
                .Include(c => c.Certifications)
                .Include(c => c.Documents)
                .Include(c => c.Gender)
                .Where(c => c.IsActive)
                .ToListAsync();
        }

        public async Task<List<CandidateProfile>> GetByIdsWithDetailsAsync(IEnumerable<long> candidateProfileIds)
        {
            var idSet = candidateProfileIds.ToList();
            return await _dbSet
                .Include(c => c.Educations).ThenInclude(e => e.Degree)
                .Include(c => c.Educations).ThenInclude(e => e.MajorSubjectSscHsc)
                .Include(c => c.Educations).ThenInclude(e => e.MajorSubjectUniversity)
                .Include(c => c.WorkExperiences)
                .Include(c => c.Skills)
                .Include(c => c.Certifications)
                .Include(c => c.Gender)
                .Where(c => idSet.Contains(c.CandidateProfileId))
                .ToListAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class ProfileFieldConfigRepository : Repository<ProfileFieldConfig>, IProfileFieldConfigRepository
    {
        public ProfileFieldConfigRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsAsync(CandidateProfileFieldEnum field, long? jobPostingId, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(c =>
                c.Field == field &&
                c.JobPostingId == jobPostingId &&
                (!excludeId.HasValue || c.ProfileFieldConfigId != excludeId.Value));
        }

        public async Task<List<ProfileFieldConfig>> GetGlobalAndForJobPostingAsync(long? jobPostingId)
        {
            return await _dbSet
                .Where(c => c.JobPostingId == null || c.JobPostingId == jobPostingId)
                .ToListAsync();
        }
    }
}

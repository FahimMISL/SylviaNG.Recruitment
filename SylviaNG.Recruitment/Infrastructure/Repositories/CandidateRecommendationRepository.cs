using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class CandidateRecommendationRepository : Repository<CandidateRecommendation>, ICandidateRecommendationRepository
    {
        public CandidateRecommendationRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<CandidateRecommendation?> GetLatestByJobApplicationIdAsync(long jobApplicationId)
        {
            return await _dbSet
                .Where(r => r.JobApplicationId == jobApplicationId)
                .OrderByDescending(r => r.RecommendedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<List<CandidateRecommendation>> GetPendingWithApplicationAsync()
        {
            return await _dbSet
                .Include(r => r.JobApplication)
                    .ThenInclude(a => a.JobPosting)
                .Where(r => r.Status == RecommendationStatusEnum.Pending)
                .OrderBy(r => r.RecommendedAt)
                .ToListAsync();
        }
    }
}

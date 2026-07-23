using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class CandidateTagRepository : Repository<CandidateTag>, ICandidateTagRepository
    {
        public CandidateTagRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<CandidateTag>> GetAllByCandidateProfileIdAsync(long candidateProfileId)
        {
            return await _dbSet.Where(t => t.CandidateProfileId == candidateProfileId).ToListAsync();
        }

        public async Task<List<string>> GetDistinctTagNamesAsync(string? search, int limit)
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(t => t.TagName.Contains(search));

            return await query
                .Select(t => t.TagName)
                .Distinct()
                .OrderBy(name => name)
                .Take(limit)
                .ToListAsync();
        }
    }
}

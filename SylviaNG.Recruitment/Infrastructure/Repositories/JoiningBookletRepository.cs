using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class JoiningBookletRepository : Repository<JoiningBooklet>, IJoiningBookletRepository
    {
        public JoiningBookletRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<JoiningBooklet>> GetAllOrderedAsync(long? jobApplicationId)
        {
            var query = _dbSet
                .Include(j => j.JobApplication)
                .Include(j => j.OfferLetter)
                .Include(j => j.DocumentTemplate)
                .AsQueryable();

            if (jobApplicationId.HasValue)
                query = query.Where(j => j.JobApplicationId == jobApplicationId.Value);

            return await query.OrderByDescending(j => j.GeneratedAt).ToListAsync();
        }

        public async Task<JoiningBooklet?> GetByIdWithDetailsAsync(long joiningBookletId)
        {
            return await _dbSet
                .Include(j => j.JobApplication)
                .Include(j => j.OfferLetter)
                .Include(j => j.DocumentTemplate)
                .FirstOrDefaultAsync(j => j.JoiningBookletId == joiningBookletId);
        }

        public async Task<List<JoiningBooklet>> GetByIdsWithDetailsAsync(List<long> joiningBookletIds)
        {
            return await _dbSet
                .Include(j => j.JobApplication)
                .Include(j => j.OfferLetter)
                .Include(j => j.DocumentTemplate)
                .Where(j => joiningBookletIds.Contains(j.JoiningBookletId))
                .ToListAsync();
        }
    }
}

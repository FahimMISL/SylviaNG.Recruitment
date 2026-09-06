using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class TargetLetterRepository : Repository<TargetLetter>, ITargetLetterRepository
    {
        public TargetLetterRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<TargetLetter>> GetAllOrderedAsync(long? jobApplicationId)
        {
            var query = _dbSet
                .Include(t => t.JobApplication)
                .Include(t => t.OfferLetter)
                .Include(t => t.DocumentTemplate)
                .AsQueryable();

            if (jobApplicationId.HasValue)
                query = query.Where(t => t.JobApplicationId == jobApplicationId.Value);

            return await query.OrderByDescending(t => t.GeneratedAt).ToListAsync();
        }

        public async Task<TargetLetter?> GetByIdWithDetailsAsync(long targetLetterId)
        {
            return await _dbSet
                .Include(t => t.JobApplication)
                .Include(t => t.OfferLetter)
                .Include(t => t.DocumentTemplate)
                .FirstOrDefaultAsync(t => t.TargetLetterId == targetLetterId);
        }
    }
}

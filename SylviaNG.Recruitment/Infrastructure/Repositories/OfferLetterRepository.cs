using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class OfferLetterRepository : Repository<OfferLetter>, IOfferLetterRepository
    {
        public OfferLetterRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<OfferLetter>> GetAllOrderedAsync(long? jobApplicationId)
        {
            var query = _dbSet
                .Include(o => o.JobApplication)
                .Include(o => o.DocumentTemplate)
                .AsQueryable();

            if (jobApplicationId.HasValue)
                query = query.Where(o => o.JobApplicationId == jobApplicationId.Value);

            return await query.OrderByDescending(o => o.GeneratedAt).ToListAsync();
        }

        public async Task<OfferLetter?> GetByIdWithDetailsAsync(long offerLetterId)
        {
            return await _dbSet
                .Include(o => o.JobApplication)
                .Include(o => o.DocumentTemplate)
                .FirstOrDefaultAsync(o => o.OfferLetterId == offerLetterId);
        }
    }
}

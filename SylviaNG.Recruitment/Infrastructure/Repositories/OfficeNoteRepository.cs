using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class OfficeNoteRepository : Repository<OfficeNote>, IOfficeNoteRepository
    {
        public OfficeNoteRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<OfficeNote>> GetAllOrderedAsync(long? jobApplicationId)
        {
            var query = _dbSet
                .Include(o => o.JobApplication)
                .Include(o => o.DocumentTemplate)
                .AsQueryable();

            if (jobApplicationId.HasValue)
                query = query.Where(o => o.JobApplicationId == jobApplicationId.Value);

            return await query.OrderByDescending(o => o.GeneratedAt).ToListAsync();
        }

        public async Task<OfficeNote?> GetByIdWithDetailsAsync(long officeNoteId)
        {
            return await _dbSet
                .Include(o => o.JobApplication)
                .Include(o => o.DocumentTemplate)
                .FirstOrDefaultAsync(o => o.OfficeNoteId == officeNoteId);
        }
    }
}

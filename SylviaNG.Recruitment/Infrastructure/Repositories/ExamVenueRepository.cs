using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class ExamVenueRepository : Repository<ExamVenue>, IExamVenueRepository
    {
        public ExamVenueRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(v => v.VenueName == name && (!excludeId.HasValue || v.ExamVenueId != excludeId.Value));
        }

        public async Task<ExamVenue?> GetByIdWithRoomsAsync(long examVenueId)
        {
            return await _dbSet
                .Include(v => v.Rooms)
                .FirstOrDefaultAsync(v => v.ExamVenueId == examVenueId);
        }

        public async Task<List<ExamVenue>> GetAllWithRoomsAsync()
        {
            return await _dbSet
                .Include(v => v.Rooms)
                .ToListAsync();
        }

        public async Task<List<ExamVenue>> GetActiveAsync()
        {
            return await _dbSet
                .Where(v => v.IsActive)
                .OrderBy(v => v.VenueName)
                .ToListAsync();
        }
    }
}

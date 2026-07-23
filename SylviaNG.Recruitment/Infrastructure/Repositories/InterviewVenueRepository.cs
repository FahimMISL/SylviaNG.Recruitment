using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class InterviewVenueRepository : Repository<InterviewVenue>, IInterviewVenueRepository
    {
        public InterviewVenueRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(v => v.VenueName == name && (!excludeId.HasValue || v.InterviewVenueId != excludeId.Value));
        }

        public async Task<InterviewVenue?> GetByIdWithRoomsAsync(long interviewVenueId)
        {
            return await _dbSet
                .Include(v => v.Rooms)
                .FirstOrDefaultAsync(v => v.InterviewVenueId == interviewVenueId);
        }

        public async Task<List<InterviewVenue>> GetAllWithRoomsAsync()
        {
            return await _dbSet
                .Include(v => v.Rooms)
                .ToListAsync();
        }

        public async Task<List<InterviewVenue>> GetActiveAsync()
        {
            return await _dbSet
                .Where(v => v.IsActive)
                .OrderBy(v => v.VenueName)
                .ToListAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class InterviewRoomRepository : Repository<InterviewRoom>, IInterviewRoomRepository
    {
        public InterviewRoomRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(long interviewVenueId, string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(r =>
                r.InterviewVenueId == interviewVenueId &&
                r.RoomName == name &&
                (!excludeId.HasValue || r.InterviewRoomId != excludeId.Value));
        }

        public async Task<List<InterviewRoom>> GetAllByVenueIdAsync(long interviewVenueId)
        {
            return await _dbSet
                .Where(r => r.InterviewVenueId == interviewVenueId)
                .OrderBy(r => r.RoomName)
                .ToListAsync();
        }

        public async Task<List<InterviewRoom>> GetActiveByVenueIdAsync(long interviewVenueId)
        {
            return await _dbSet
                .Where(r => r.InterviewVenueId == interviewVenueId && r.IsActive)
                .OrderBy(r => r.RoomName)
                .ToListAsync();
        }

        public async Task<int> GetRoomCountByVenueIdAsync(long interviewVenueId)
        {
            return await _dbSet.CountAsync(r => r.InterviewVenueId == interviewVenueId);
        }
    }
}

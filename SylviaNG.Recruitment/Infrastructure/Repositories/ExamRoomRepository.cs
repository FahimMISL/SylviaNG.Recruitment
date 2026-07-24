using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class ExamRoomRepository : Repository<ExamRoom>, IExamRoomRepository
    {
        public ExamRoomRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(long examVenueId, string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(r =>
                r.ExamVenueId == examVenueId &&
                r.RoomName == name &&
                (!excludeId.HasValue || r.ExamRoomId != excludeId.Value));
        }

        public async Task<List<ExamRoom>> GetAllByVenueIdAsync(long examVenueId)
        {
            return await _dbSet
                .Where(r => r.ExamVenueId == examVenueId)
                .OrderBy(r => r.RoomName)
                .ToListAsync();
        }

        public async Task<List<ExamRoom>> GetActiveByVenueIdAsync(long examVenueId)
        {
            return await _dbSet
                .Where(r => r.ExamVenueId == examVenueId && r.IsActive)
                .OrderBy(r => r.RoomName)
                .ToListAsync();
        }

        public async Task<int> GetRoomCountByVenueIdAsync(long examVenueId)
        {
            return await _dbSet.CountAsync(r => r.ExamVenueId == examVenueId);
        }
    }
}

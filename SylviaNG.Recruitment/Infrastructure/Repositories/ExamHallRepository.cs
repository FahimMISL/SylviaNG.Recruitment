using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class ExamHallRepository : Repository<ExamHall>, IExamHallRepository
    {
        public ExamHallRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(h => h.HallName == name && (!excludeId.HasValue || h.ExamHallId != excludeId.Value));
        }

        public async Task<ExamHall?> GetByIdWithInvigilatorsAsync(long examHallId)
        {
            return await _dbSet
                .Include(h => h.Invigilators)
                .FirstOrDefaultAsync(h => h.ExamHallId == examHallId);
        }

        public async Task<List<ExamHall>> GetAllWithInvigilatorsAsync()
        {
            return await _dbSet
                .Include(h => h.Invigilators)
                .ToListAsync();
        }

        public async Task<List<ExamHall>> GetActiveAsync()
        {
            return await _dbSet
                .Where(h => h.IsActive)
                .OrderBy(h => h.HallName)
                .ToListAsync();
        }

        public async Task<HashSet<long>> GetExistingEmployeeIdsAsync(IEnumerable<long> employeeIds)
        {
            var ids = employeeIds.ToList();
            var existing = await _dbContext.Employees
                .Where(e => ids.Contains(e.EmployeeId))
                .Select(e => e.EmployeeId)
                .ToListAsync();
            return existing.ToHashSet();
        }
    }
}

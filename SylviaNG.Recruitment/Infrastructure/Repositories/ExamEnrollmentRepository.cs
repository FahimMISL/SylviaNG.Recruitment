using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class ExamEnrollmentRepository : Repository<ExamEnrollment>, IExamEnrollmentRepository
    {
        public ExamEnrollmentRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByExamAndJobApplicationAsync(long examId, long jobApplicationId)
        {
            return await _dbSet.AnyAsync(e => e.ExamId == examId && e.JobApplicationId == jobApplicationId);
        }

        public async Task<bool> ExistsBySeatNumberAsync(long examId, string seatNumber, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(e =>
                e.ExamId == examId &&
                e.SeatNumber == seatNumber &&
                (!excludeId.HasValue || e.ExamEnrollmentId != excludeId.Value));
        }

        public async Task<List<ExamEnrollment>> GetByExamIdAsync(long examId)
        {
            return await _dbSet
                .Include(e => e.JobApplication)
                .Include(e => e.ExamRoom)
                .Where(e => e.ExamId == examId)
                .OrderBy(e => e.ExamRoomId)
                .ThenBy(e => e.SeatNumber)
                .ToListAsync();
        }

        public async Task<ExamEnrollment?> GetByIdWithDetailsAsync(long examEnrollmentId)
        {
            return await _dbSet
                .Include(e => e.Exam)
                    .ThenInclude(e => e.ExamVenue)
                .Include(e => e.JobApplication)
                .Include(e => e.ExamRoom)
                .FirstOrDefaultAsync(e => e.ExamEnrollmentId == examEnrollmentId);
        }

        public async Task<int> GetCountByRoomIdAsync(long examRoomId)
        {
            return await _dbSet.CountAsync(e => e.ExamRoomId == examRoomId);
        }
    }
}

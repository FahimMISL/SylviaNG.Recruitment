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
                    .ThenInclude(ja => ja.CandidateProfile)
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
                    .ThenInclude(ja => ja.CandidateProfile)
                .Include(e => e.ExamRoom)
                .FirstOrDefaultAsync(e => e.ExamEnrollmentId == examEnrollmentId);
        }

        public async Task<int> GetCountByRoomIdAsync(long examRoomId)
        {
            return await _dbSet.CountAsync(e => e.ExamRoomId == examRoomId);
        }

        public async Task<List<ExamEnrollment>> GetByCandidateProfileIdAsync(long candidateProfileId)
        {
            return await _dbSet
                .Include(e => e.Exam)
                    .ThenInclude(e => e.JobPosting)
                .Include(e => e.JobApplication)
                .Where(e => e.JobApplication.CandidateProfileId == candidateProfileId)
                .OrderByDescending(e => e.Exam.ScheduledStartAt)
                .ToListAsync();
        }

        public async Task<ExamEnrollment?> GetByIdWithExamAndQuestionsAsync(long examEnrollmentId)
        {
            return await _dbSet
                .Include(e => e.Exam)
                .Include(e => e.JobApplication)
                .Include(e => e.Answers)
                .FirstOrDefaultAsync(e => e.ExamEnrollmentId == examEnrollmentId);
        }

        public async Task<List<ExamEnrollment>> GetByExamIdWithDetailsAsync(long examId)
        {
            return await _dbSet
                .Include(e => e.Exam)
                    .ThenInclude(e => e.ExamVenue)
                .Include(e => e.JobApplication)
                    .ThenInclude(ja => ja.CandidateProfile)
                .Include(e => e.ExamRoom)
                .Where(e => e.ExamId == examId)
                .OrderBy(e => e.ExamRoomId)
                .ThenBy(e => e.SeatNumber)
                .ToListAsync();
        }
    }
}

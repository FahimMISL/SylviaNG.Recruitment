using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IExamEnrollmentRepository : IRepository<ExamEnrollment>
    {
        Task<bool> ExistsByExamAndJobApplicationAsync(long examId, long jobApplicationId);

        /// <summary>Seat-number uniqueness is scoped to a single exam - app-layer check, no DB constraint, same convention as ExamRoom.RoomName.</summary>
        Task<bool> ExistsBySeatNumberAsync(long examId, string seatNumber, long? excludeId = null);

        Task<List<ExamEnrollment>> GetByExamIdAsync(long examId);

        /// <summary>Single enrollment with Exam/JobApplication/ExamRoom included, for admit-card generation and reassignment.</summary>
        Task<ExamEnrollment?> GetByIdWithDetailsAsync(long examEnrollmentId);

        Task<int> GetCountByRoomIdAsync(long examRoomId);
    }
}

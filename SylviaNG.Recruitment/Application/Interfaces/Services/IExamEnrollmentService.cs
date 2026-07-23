using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IExamEnrollmentService
    {
        Task<List<long>> EnrollAsync(long examId, List<long> jobApplicationIds);
        Task<List<ExamEnrollmentResponse>> GetByExamIdAsync(long examId);
        Task ReassignSeatAsync(long examEnrollmentId, long examRoomId, string seatNumber);

        /// <summary>HR manually uploads/overwrites a single enrollment's score (US-059 AC1) -
        /// also the finalization path for exams with Subjective questions the auto-scorer left
        /// ungraded (US-058 AC3).</summary>
        Task UploadScoreAsync(long examEnrollmentId, decimal score);
    }
}

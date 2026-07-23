using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IExamEnrollmentService
    {
        Task<List<long>> EnrollAsync(long examId, List<long> jobApplicationIds);
        Task<List<ExamEnrollmentResponse>> GetByExamIdAsync(long examId);
        Task ReassignSeatAsync(long examEnrollmentId, long examRoomId, string seatNumber);
    }
}

using SylviaNG.Recruitment.Application.Features.ExamTaking.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>US-058: the current candidate's own online exam attempts - list, start, submit.</summary>
    public interface IExamTakingService
    {
        Task<List<MyExamEnrollmentResponse>> GetMyEnrollmentsAsync();
        Task<ExamPaperResponse> StartExamAsync(long examEnrollmentId);
        Task<ExamSubmitResultResponse> SubmitExamAsync(long examEnrollmentId, ExamSubmitRequest request);
    }
}

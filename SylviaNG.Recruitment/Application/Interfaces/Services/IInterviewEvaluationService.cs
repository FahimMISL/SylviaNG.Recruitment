using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IInterviewEvaluationService
    {
        Task<long> SubmitAsync(long interviewId, InterviewEvaluationSubmitRequest request);
        Task UpdateAsync(long interviewEvaluationId, InterviewEvaluationUpdateRequest request);
        Task<InterviewEvaluationResponse> GetByIdAsync(long interviewEvaluationId);
        Task<List<InterviewEvaluationResponse>> GetByInterviewIdAsync(long interviewId);
        Task<InterviewEvaluationResultsFileResponse> ExportResultsExcelAsync(long interviewId);
    }
}

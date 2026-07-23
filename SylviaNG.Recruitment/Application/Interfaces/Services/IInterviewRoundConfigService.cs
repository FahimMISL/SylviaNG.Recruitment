using SylviaNG.Recruitment.Application.Features.InterviewRoundConfigs.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IInterviewRoundConfigService
    {
        Task<List<InterviewRoundConfigResponse>> GetAllByJobPostingIdAsync(long jobPostingId);

        /// <summary>Atomically replaces every round configured for a job posting with the submitted
        /// list (US-070 AC1/AC4) - existing rounds not present in the list are removed unless an
        /// Interview already references them, in which case removal is rejected.</summary>
        Task ReplaceForJobPostingAsync(long jobPostingId, InterviewRoundConfigReplaceRequest request);
    }
}

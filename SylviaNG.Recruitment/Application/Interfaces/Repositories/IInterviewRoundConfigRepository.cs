using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IInterviewRoundConfigRepository : IRepository<InterviewRoundConfig>
    {
        /// <summary>All rounds configured for a job posting, ordered by Sequence, with
        /// PanelMembers and Scorecard included.</summary>
        Task<List<InterviewRoundConfig>> GetAllByJobPostingIdAsync(long jobPostingId);

        Task<InterviewRoundConfig?> GetByIdWithDetailsAsync(long interviewRoundConfigId);

        Task<InterviewRoundConfig?> GetByJobPostingAndSequenceAsync(long jobPostingId, int sequence);
    }
}

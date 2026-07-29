using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IInterviewEvaluationRepository : IRepository<InterviewEvaluation>
    {
        Task<bool> ExistsByInterviewAndEmployeeAsync(long interviewId, long employeeId, long? excludeId = null);

        /// <summary>Every evaluation for one interview, with Scores+ScorecardCriterion and the
        /// Scorecard itself included, for the aggregated results view.</summary>
        Task<List<InterviewEvaluation>> GetByInterviewIdAsync(long interviewId);

        /// <summary>Single evaluation with Scores+ScorecardCriterion included, for edit/detail.</summary>
        Task<InterviewEvaluation?> GetByIdWithDetailsAsync(long interviewEvaluationId);

        /// <summary>EP-14 US-110: every evaluation in scope, with Scores+ScorecardCriterion (for
        /// WeightedScore) and Interview.JobApplication.JobPosting (for vacancy/department
        /// filtering) included.</summary>
        Task<List<InterviewEvaluation>> GetForAnalyticsScopeAsync(
            long? jobPostingId, long? departmentId, DateTime? dateFrom, DateTime? dateTo);
    }
}

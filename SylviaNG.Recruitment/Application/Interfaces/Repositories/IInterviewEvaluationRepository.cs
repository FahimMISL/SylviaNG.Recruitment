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
    }
}

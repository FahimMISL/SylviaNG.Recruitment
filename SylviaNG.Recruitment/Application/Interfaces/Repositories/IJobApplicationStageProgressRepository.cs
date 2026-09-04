using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IJobApplicationStageProgressRepository : IRepository<JobApplicationStageProgress>
    {
        /// <summary>All stage-progress rows for one application, ordered by DisplayOrder (US-042).</summary>
        Task<List<JobApplicationStageProgress>> GetByJobApplicationIdAsync(long jobApplicationId);

        /// <summary>EP-14 US-109: the "current" stage-progress row per application - the InProgress
        /// row if one exists, else the highest-DisplayOrder Completed row. Applications with
        /// neither (brand-new, everything still Pending) are simply absent from the result.</summary>
        Task<Dictionary<long, JobApplicationStageProgress>> GetCurrentByJobApplicationIdsAsync(List<long> jobApplicationIds);

        /// <summary>EP-14 US-105: count of applications currently sitting at a stage that requires
        /// manual approval to advance ("Pending Approvals" dashboard metric).</summary>
        Task<int> CountPendingApprovalsAsync();
    }
}

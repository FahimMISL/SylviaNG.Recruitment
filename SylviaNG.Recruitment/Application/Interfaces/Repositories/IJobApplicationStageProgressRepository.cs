using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IJobApplicationStageProgressRepository : IRepository<JobApplicationStageProgress>
    {
        /// <summary>All stage-progress rows for one application, ordered by DisplayOrder (US-042).</summary>
        Task<List<JobApplicationStageProgress>> GetByJobApplicationIdAsync(long jobApplicationId);
    }
}

using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IHiringPipelineRepository : IRepository<HiringPipeline>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);

        /// <summary>Full aggregate load: pipeline, its stages (ordered), each stage's interviewers, and job posting count.</summary>
        Task<HiringPipeline?> GetByIdWithStagesAsync(long hiringPipelineId);
        Task<List<HiringPipeline>> GetAllWithStagesAsync();

        /// <summary>Active pipelines only (Id + Name), for the job-posting "assign pipeline" dropdown.</summary>
        Task<List<HiringPipeline>> GetActiveAsync();

        /// <summary>Which of the given employee IDs actually exist, for validating stage interviewer assignments.</summary>
        Task<HashSet<long>> GetExistingEmployeeIdsAsync(IEnumerable<long> employeeIds);

        /// <summary>Count of active pipelines, for dashboard summary stats.</summary>
        Task<int> CountActiveAsync();
    }
}

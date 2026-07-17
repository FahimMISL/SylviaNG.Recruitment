using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IAssessmentWorkflowRepository : IRepository<AssessmentWorkflow>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);

        /// <summary>Full aggregate load: workflow, its stages (ordered), and job posting count.</summary>
        Task<AssessmentWorkflow?> GetByIdWithStagesAsync(long assessmentWorkflowId);
        Task<List<AssessmentWorkflow>> GetAllWithStagesAsync();

        /// <summary>Active workflows only, for the job-posting "assign workflow" dropdown.</summary>
        Task<List<AssessmentWorkflow>> GetActiveAsync();
    }
}

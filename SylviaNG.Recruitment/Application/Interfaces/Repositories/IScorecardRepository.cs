using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IScorecardRepository : IRepository<Scorecard>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);

        /// <summary>Full aggregate load: scorecard and its criteria (ordered), for edit/evaluate.</summary>
        Task<Scorecard?> GetByIdWithCriteriaAsync(long scorecardId);
        Task<List<Scorecard>> GetAllWithCriteriaAsync();

        /// <summary>Active scorecards only (Id + Name), for a "pick a scorecard" dropdown.</summary>
        Task<List<Scorecard>> GetActiveAsync();
    }
}

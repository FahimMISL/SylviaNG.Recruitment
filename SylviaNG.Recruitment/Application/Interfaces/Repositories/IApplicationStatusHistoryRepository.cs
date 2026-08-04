using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IApplicationStatusHistoryRepository : IRepository<ApplicationStatusHistory>
    {
        /// <summary>EP-14 US-106/US-107: every status-history row for the given JobApplicationIds,
        /// with Reason included, ordered by JobApplicationId then ChangedAt - feeds both the funnel
        /// stage/conversion/drop-off-reason aggregation and the time-to-hire per-stage duration
        /// breakdown (consecutive ChangedAt deltas).</summary>
        Task<List<ApplicationStatusHistory>> GetForApplicationsAsync(IEnumerable<long> jobApplicationIds);
    }
}

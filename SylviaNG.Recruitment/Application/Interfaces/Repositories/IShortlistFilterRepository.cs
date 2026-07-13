using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IShortlistFilterRepository : IRepository<ShortlistFilter>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);

        /// <summary>Full aggregate load: filter and its criteria (ordered), for edit/preview (US-043).</summary>
        Task<ShortlistFilter?> GetByIdWithCriteriaAsync(long shortlistFilterId);
        Task<List<ShortlistFilter>> GetAllWithCriteriaAsync();

        /// <summary>Active filters only (Id + Name), for a "pick a saved filter" dropdown.</summary>
        Task<List<ShortlistFilter>> GetActiveAsync();
    }
}

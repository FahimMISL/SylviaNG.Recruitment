using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface ISavedSearchRepository : IRepository<SavedSearch>
    {
        Task<bool> ExistsByNameForOwnerAsync(string ownerUserName, string name, long? excludeId = null);

        /// <summary>Current user's own searches plus every shared search, for the dropdown/manage dialog (AC2/AC4).</summary>
        Task<List<SavedSearch>> GetVisibleAsync(string ownerUserName);
    }
}

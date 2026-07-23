using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IUniversityLibraryItemRepository : IRepository<UniversityLibraryItem>
    {
        Task<List<UniversityLibraryItem>> GetAllOrderedAsync();
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
        Task<int> CountUsageAsync(long universityLibraryItemId);
    }
}

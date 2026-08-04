using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface ISpecialCategoryRepository : IRepository<SpecialCategory>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
        Task<List<SpecialCategory>> GetAllOrderedAsync();
        Task<int> CountUsageAsync(long specialCategoryId);
    }
}

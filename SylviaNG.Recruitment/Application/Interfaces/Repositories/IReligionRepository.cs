using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IReligionRepository : IRepository<Religion>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
        Task<List<Religion>> GetAllOrderedAsync();
        Task<int> CountUsageAsync(long religionId);
    }
}

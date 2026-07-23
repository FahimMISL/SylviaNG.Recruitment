using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IBloodGroupRepository : IRepository<BloodGroup>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
        Task<List<BloodGroup>> GetAllOrderedAsync();
        Task<int> CountUsageAsync(long bloodGroupId);
    }
}

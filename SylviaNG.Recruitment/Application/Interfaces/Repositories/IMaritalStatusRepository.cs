using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IMaritalStatusRepository : IRepository<MaritalStatus>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
        Task<List<MaritalStatus>> GetAllOrderedAsync();
        Task<int> CountUsageAsync(long maritalStatusId);
    }
}

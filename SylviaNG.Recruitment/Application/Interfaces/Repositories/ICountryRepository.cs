using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface ICountryRepository : IRepository<Country>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
        Task<bool> ExistsByCodeAsync(string code, long? excludeId = null);
        Task<List<Country>> GetAllOrderedAsync();
        Task<int> CountUsageAsync(long countryId);
    }
}

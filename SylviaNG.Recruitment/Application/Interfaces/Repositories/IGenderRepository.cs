using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IGenderRepository : IRepository<Gender>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
        Task<List<Gender>> GetAllOrderedAsync();
        Task<int> CountUsageAsync(long genderId);
    }
}

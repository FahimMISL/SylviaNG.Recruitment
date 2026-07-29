using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IDegreeRepository : IRepository<Degree>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
        Task<List<Degree>> GetAllOrderedAsync();
        Task<int> CountUsageAsync(long degreeId);
    }
}

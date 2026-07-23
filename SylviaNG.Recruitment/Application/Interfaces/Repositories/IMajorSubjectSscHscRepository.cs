using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IMajorSubjectSscHscRepository : IRepository<MajorSubjectSscHsc>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
        Task<List<MajorSubjectSscHsc>> GetAllOrderedAsync();
        Task<int> CountUsageAsync(long majorSubjectSscHscId);
    }
}

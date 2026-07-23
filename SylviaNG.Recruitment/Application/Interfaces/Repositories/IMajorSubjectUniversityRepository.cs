using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IMajorSubjectUniversityRepository : IRepository<MajorSubjectUniversity>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
        Task<List<MajorSubjectUniversity>> GetAllOrderedAsync();
        Task<int> CountUsageAsync(long majorSubjectUniversityId);
    }
}

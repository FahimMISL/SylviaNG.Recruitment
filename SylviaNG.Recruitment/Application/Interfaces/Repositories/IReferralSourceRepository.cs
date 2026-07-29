using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IReferralSourceRepository : IRepository<ReferralSource>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
        Task<List<ReferralSource>> GetAllOrderedAsync();
        Task<int> CountUsageAsync(long referralSourceId);
    }
}

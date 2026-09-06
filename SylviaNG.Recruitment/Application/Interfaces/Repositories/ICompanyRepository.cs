using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface ICompanyRepository : IRepository<Company>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
        Task<List<Company>> GetAllOrderedAsync();

        /// <summary>Counts of JobPostings/UserAccounts per company, for the Companies list's
        /// "basic recruitment statistics" column. Bypasses the ICompanyScoped filter (SuperAdmin
        /// viewing across every company) since callers already restrict this endpoint to SuperAdmin.</summary>
        Task<(int JobPostingCount, int UserAccountCount)> GetStatsAsync(long companyId);
    }
}

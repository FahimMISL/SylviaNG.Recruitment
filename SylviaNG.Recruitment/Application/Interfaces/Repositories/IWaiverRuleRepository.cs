using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IWaiverRuleRepository : IRepository<WaiverRule>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);

        /// <summary>All rules with SpecialCategory/ReferralSource included, ordered Priority then Name, for the admin list.</summary>
        Task<List<WaiverRule>> GetAllOrderedAsync();

        /// <summary>Active rules only, ordered Priority ascending then WaiverRuleId ascending, for submit-time matching.</summary>
        Task<List<WaiverRule>> GetActiveOrderedByPriorityAsync();
    }
}

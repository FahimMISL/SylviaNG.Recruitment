using SylviaNG.Recruitment.Application.Features.WaiverRules.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IWaiverRuleService
    {
        Task<long> CreateAsync(WaiverRuleCreateRequest request);
        Task UpdateAsync(long waiverRuleId, WaiverRuleUpdateRequest request);
        Task DeleteAsync(long waiverRuleId);
        Task<List<WaiverRuleResponse>> GetAllAsync();

        /// <summary>
        /// EP-17/US-127: first ACTIVE rule (Priority ascending, then WaiverRuleId ascending) whose
        /// every non-null criterion matches the submitting candidate, or null if none match.
        /// </summary>
        Task<WaiverRule?> TryMatchAsync(bool candidateIsInternal, long? specialCategoryId, long? referralSourceId);
    }
}

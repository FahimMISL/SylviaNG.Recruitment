using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.Referrals.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IReferralService
    {
        Task<long> CreateAsync(ReferralCreateRequest request);
        Task UpdateAsync(long referralId, ReferralUpdateRequest request);
        Task DeleteAsync(long referralId);
        Task<List<ReferralResponse>> GetAllAsync();
        Task<ReferralResponse> GetByIdAsync(long referralId);
        Task<PagedResult<ReferralResponse>> GetPaginatedAsync(PagedRequest request);
    }
}

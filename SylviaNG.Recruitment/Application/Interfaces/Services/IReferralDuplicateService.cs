using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IReferralDuplicateService
    {
        Task<long> CreateAsync(ReferralDuplicateCreateRequest request);
        Task UpdateAsync(long referralDuplicateId, ReferralDuplicateUpdateRequest request);
        Task DeleteAsync(long referralDuplicateId);
        Task<List<ReferralDuplicateResponse>> GetAllAsync();
        Task<ReferralDuplicateResponse> GetByIdAsync(long referralDuplicateId);
        Task<PagedResult<ReferralDuplicateResponse>> GetPaginatedAsync(PagedRequest request);
    }
}

using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.OnboardingRecords.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IOnboardingRecordService
    {
        Task<long> CreateAsync(OnboardingRecordCreateRequest request);
        Task UpdateAsync(long onboardingRecordId, OnboardingRecordUpdateRequest request);
        Task DeleteAsync(long onboardingRecordId);
        Task<List<OnboardingRecordResponse>> GetAllAsync();
        Task<OnboardingRecordResponse> GetByIdAsync(long onboardingRecordId);
        Task<PagedResult<OnboardingRecordResponse>> GetPaginatedAsync(PagedRequest request);
    }
}

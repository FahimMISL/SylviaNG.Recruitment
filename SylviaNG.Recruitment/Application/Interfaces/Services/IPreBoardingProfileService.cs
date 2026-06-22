using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IPreBoardingProfileService
    {
        Task<long> CreateAsync(PreBoardingProfileCreateRequest request);
        Task UpdateAsync(long preBoardingProfileId, PreBoardingProfileUpdateRequest request);
        Task DeleteAsync(long preBoardingProfileId);
        Task<List<PreBoardingProfileResponse>> GetAllAsync();
        Task<PreBoardingProfileResponse> GetByIdAsync(long preBoardingProfileId);
        Task<PagedResult<PreBoardingProfileResponse>> GetPaginatedAsync(PagedRequest request);
    }
}

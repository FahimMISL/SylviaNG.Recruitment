using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IRecruitmentAgencyService
    {
        Task<long> CreateAsync(RecruitmentAgencyCreateRequest request);
        Task UpdateAsync(long recruitmentAgencyId, RecruitmentAgencyUpdateRequest request);
        Task DeleteAsync(long recruitmentAgencyId);
        Task<List<RecruitmentAgencyResponse>> GetAllAsync();
        Task<RecruitmentAgencyResponse> GetByIdAsync(long recruitmentAgencyId);
        Task<PagedResult<RecruitmentAgencyResponse>> GetPaginatedAsync(PagedRequest request);
    }
}

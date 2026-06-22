using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.AdmitCards.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IAdmitCardService
    {
        Task<long> CreateAsync(AdmitCardCreateRequest request);
        Task UpdateAsync(long admitCardId, AdmitCardUpdateRequest request);
        Task DeleteAsync(long admitCardId);
        Task<List<AdmitCardResponse>> GetAllAsync();
        Task<AdmitCardResponse> GetByIdAsync(long admitCardId);
        Task<PagedResult<AdmitCardResponse>> GetPaginatedAsync(PagedRequest request);
    }
}

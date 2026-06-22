using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.OfferCompensations.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IOfferCompensationService
    {
        Task<long> CreateAsync(OfferCompensationCreateRequest request);
        Task UpdateAsync(long offerCompensationId, OfferCompensationUpdateRequest request);
        Task DeleteAsync(long offerCompensationId);
        Task<List<OfferCompensationResponse>> GetAllAsync();
        Task<OfferCompensationResponse> GetByIdAsync(long offerCompensationId);
        Task<PagedResult<OfferCompensationResponse>> GetPaginatedAsync(PagedRequest request);
    }
}

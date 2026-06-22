using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.VerificationItems.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IVerificationItemService
    {
        Task<long> CreateAsync(VerificationItemCreateRequest request);
        Task UpdateAsync(long verificationItemId, VerificationItemUpdateRequest request);
        Task DeleteAsync(long verificationItemId);
        Task<List<VerificationItemResponse>> GetAllAsync();
        Task<VerificationItemResponse> GetByIdAsync(long verificationItemId);
        Task<PagedResult<VerificationItemResponse>> GetPaginatedAsync(PagedRequest request);
    }
}

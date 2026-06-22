using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.CareerContents.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ICareerContentService
    {
        Task<long> CreateAsync(CareerContentCreateRequest request);
        Task UpdateAsync(long careerContentId, CareerContentUpdateRequest request);
        Task DeleteAsync(long careerContentId);
        Task<List<CareerContentResponse>> GetAllAsync();
        Task<CareerContentResponse> GetByIdAsync(long careerContentId);
        Task<PagedResult<CareerContentResponse>> GetPaginatedAsync(PagedRequest request);
    }
}

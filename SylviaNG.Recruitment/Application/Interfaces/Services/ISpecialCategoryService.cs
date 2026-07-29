using SylviaNG.Recruitment.Application.Features.SpecialCategories.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ISpecialCategoryService
    {
        Task<long> CreateAsync(SpecialCategoryCreateRequest request);
        Task UpdateAsync(long specialCategoryId, SpecialCategoryUpdateRequest request);
        Task DeleteAsync(long specialCategoryId);
        Task<List<SpecialCategoryResponse>> GetAllAsync();
    }
}

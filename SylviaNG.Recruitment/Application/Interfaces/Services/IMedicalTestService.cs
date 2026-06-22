using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.MedicalTests.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IMedicalTestService
    {
        Task<long> CreateAsync(MedicalTestCreateRequest request);
        Task UpdateAsync(long medicalTestId, MedicalTestUpdateRequest request);
        Task DeleteAsync(long medicalTestId);
        Task<List<MedicalTestResponse>> GetAllAsync();
        Task<MedicalTestResponse> GetByIdAsync(long medicalTestId);
        Task<PagedResult<MedicalTestResponse>> GetPaginatedAsync(PagedRequest request);
    }
}

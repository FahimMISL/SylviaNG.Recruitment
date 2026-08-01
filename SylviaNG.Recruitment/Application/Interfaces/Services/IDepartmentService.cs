using SylviaNG.Recruitment.Application.Features.Departments.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IDepartmentService
    {
        Task<long> CreateAsync(DepartmentCreateRequest request);
        Task UpdateAsync(long departmentId, DepartmentUpdateRequest request);
        Task DeleteAsync(long departmentId);
        Task<List<DepartmentResponse>> GetAllAsync();
    }
}

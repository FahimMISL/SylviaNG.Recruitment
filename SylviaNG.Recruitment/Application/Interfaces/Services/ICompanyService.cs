using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Features.Companies.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>SuperAdmin-only CRUD over Company (tenant organizations). See CompanyController.</summary>
    public interface ICompanyService
    {
        Task<long> CreateAsync(CompanyCreateRequest request);
        Task UpdateAsync(long companyId, CompanyUpdateRequest request);
        Task SetActiveAsync(long companyId, bool isActive);
        Task<CompanyResponse> GetByIdAsync(long companyId);
        Task<List<CompanyResponse>> GetAllAsync();
        Task<string> UploadLogoAsync(long companyId, IFormFile file);
    }
}

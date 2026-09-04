using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface ICompanyBrandingRepository : IRepository<CompanyBranding>
    {
        /// <summary>The active branding row for the given Company (null = unrestricted/system row), or null if none created yet.</summary>
        Task<CompanyBranding?> GetByCompanyIdAsync(long? companyId);
    }
}

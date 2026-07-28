using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface ICompanyBrandingRepository : IRepository<CompanyBranding>
    {
        /// <summary>The active branding row for the given tenant, or null if none seeded yet.</summary>
        Task<CompanyBranding?> GetByTenantIdAsync(string tenantId);
    }
}

using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>EP-18 F1: resolves the active CompanyBranding for the current tenant, for use by
    /// the shared document component layer (Infrastructure/Documents/Shared).</summary>
    public interface IBrandingResolverService
    {
        /// <summary>Never returns null - falls back to an in-memory default branding if no row is
        /// seeded yet for the resolved tenant.</summary>
        Task<CompanyBranding> GetActiveBrandingAsync();
    }
}

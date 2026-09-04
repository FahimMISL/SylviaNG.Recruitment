using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Features.CompanyBranding.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>
    /// EP-18 F3: admin-facing CRUD over CompanyBranding, distinct from IBrandingResolverService
    /// (which is the read-only, tenant-resolving lookup the PDF generators use). Single-tenant
    /// today, so "current" always means the same tenant IBrandingResolverService already resolves.
    /// </summary>
    public interface ICompanyBrandingService
    {
        Task<CompanyBrandingResponse> GetAsync();

        /// <summary>Admin-only update. Creates the tenant's CompanyBranding row on first save if
        /// none was seeded yet (materializes IBrandingResolverService's in-memory default).</summary>
        Task UpdateAsync(CompanyBrandingUpdateRequest request);

        /// <summary>Saves a new logo file, replacing (and deleting) any previous one. Returns the
        /// web-relative file path.</summary>
        Task<string> UploadLogoAsync(IFormFile file);

        /// <summary>Renders a sample PDF using the shared component layer, reflecting the given
        /// (possibly unsaved) settings merged over the persisted logo/tenant - lets the admin
        /// preview draft edits before Save.</summary>
        Task<byte[]> GeneratePreviewPdfAsync(CompanyBrandingUpdateRequest request);
    }
}

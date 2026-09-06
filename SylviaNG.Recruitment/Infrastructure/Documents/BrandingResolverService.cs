using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Infrastructure.Data;

namespace SylviaNG.Recruitment.Infrastructure.Documents
{
    /// <summary>
    /// EP-18 F1/multi-tenancy follow-up: resolves the active per-Company branding row, then
    /// merges in live Company identity/contact fields (Name/Address/Phone/Email/Website) so
    /// Company is the single source of truth - Company Management edits it, Branding Settings
    /// only displays it. CompanyId is read from HttpContext.Items (same value
    /// ApplicationDBContext.CurrentCompanyId reads, resolved once per request by
    /// CompanyScopeMiddleware) rather than re-deriving it, to avoid a second DB round-trip.
    /// </summary>
    public class BrandingResolverService : IBrandingResolverService
    {
        private static readonly (string Name, string Address, string Phone, string Email) SystemDefaults =
            ("Millennium Information Solution Ltd.",
             "Administrative Building-01, Level-18, Grameen Bank Head Office, Mirpur-2, Dhaka-1216",
             "09601 789 789",
             "info@mislbd.com");

        private readonly ICompanyBrandingRepository _companyBrandingRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public BrandingResolverService(
            ICompanyBrandingRepository companyBrandingRepository,
            ICompanyRepository companyRepository,
            IHttpContextAccessor? httpContextAccessor = null)
        {
            _companyBrandingRepository = companyBrandingRepository;
            _companyRepository = companyRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CompanyBranding> GetActiveBrandingAsync()
        {
            var companyId = ResolveCompanyId();
            var branding = await _companyBrandingRepository.GetByCompanyIdAsync(companyId)
                ?? DefaultBranding(companyId);

            await ApplyCompanyIdentityAsync(branding, companyId);
            return branding;
        }

        private long? ResolveCompanyId()
            => _httpContextAccessor?.HttpContext?.Items[ApplicationDBContext.CompanyScopeItemKey] as long?;

        private async Task ApplyCompanyIdentityAsync(CompanyBranding branding, long? companyId)
        {
            var company = companyId.HasValue ? await _companyRepository.GetByIdAsync(companyId.Value) : null;

            branding.CompanyName = company?.Name ?? SystemDefaults.Name;
            branding.AddressLine = company?.Address ?? SystemDefaults.Address;
            branding.Phone = company?.Phone ?? SystemDefaults.Phone;
            branding.Email = company?.Email ?? SystemDefaults.Email;
            branding.Website = company?.Website;
        }

        // No row yet for this Company (or the unrestricted system context) - same fallback
        // convention as ApplicationSettingRepository.GetSingletonAsync().
        private static CompanyBranding DefaultBranding(long? companyId) => new()
        {
            CompanyBrandingId = 1,
            CompanyId = companyId,
            PrimaryColor = "#7A2E2E",
            SecondaryColor = "#1F2937",
            AccentColor = "#DC2626",
            FontFamily = "Helvetica",
            HeaderLayout = HeaderLayoutEnum.LogoLeftTextRight,
            FooterLayout = FooterLayoutEnum.ThreeColumn,
            MarginTop = 30,
            MarginBottom = 30,
            MarginLeft = 30,
            MarginRight = 30,
            BorderStyle = DocumentBorderStyleEnum.Solid,
            CornerRadius = 6,
            HeaderDividerStyle = DocumentDividerStyleEnum.SolidLine,
            FooterDividerStyle = DocumentDividerStyleEnum.SolidLine,
            QrCodePosition = DocumentElementPositionEnum.TopRight,
            SignaturePosition = DocumentElementPositionEnum.BottomLeft,
            SealPosition = DocumentElementPositionEnum.BottomRight,
            DocumentReferenceFormat = "{ORG}/{DOCTYPE}/{YEAR}/{SEQ}",
            ShowPageNumbers = true
        };
    }
}

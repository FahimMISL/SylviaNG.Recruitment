using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Infrastructure.Documents
{
    /// <summary>
    /// EP-18 F1. Tenant resolution here intentionally duplicates ApplicationDBContext's
    /// CurrentTenantId 3-step priority (JWT claim -> Finbuckle TenantInfo -> empty fallback)
    /// rather than extracting a shared provider - that logic is 3 lines with no side effects,
    /// and ApplicationDBContext is the most sensitive file in the codebase to touch for this.
    /// Extract only if a third consumer of this logic appears later.
    /// </summary>
    public class BrandingResolverService : IBrandingResolverService
    {
        private readonly ICompanyBrandingRepository _companyBrandingRepository;
        private readonly IMultiTenantContextAccessor<MultiTenancy.TenantInfo>? _multiTenantContextAccessor;
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public BrandingResolverService(
            ICompanyBrandingRepository companyBrandingRepository,
            IMultiTenantContextAccessor<MultiTenancy.TenantInfo>? multiTenantContextAccessor = null,
            IHttpContextAccessor? httpContextAccessor = null)
        {
            _companyBrandingRepository = companyBrandingRepository;
            _multiTenantContextAccessor = multiTenantContextAccessor;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CompanyBranding> GetActiveBrandingAsync()
        {
            var tenantId = ResolveTenantId();
            return await _companyBrandingRepository.GetByTenantIdAsync(tenantId)
                ?? DefaultBranding(tenantId);
        }

        private string ResolveTenantId()
        {
            // Priority 1: JWT claims from HTTP request
            if (_httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var tenantClaim = _httpContextAccessor.HttpContext.User.FindFirst("tenant_id");
                if (tenantClaim != null && !string.IsNullOrEmpty(tenantClaim.Value))
                {
                    return tenantClaim.Value;
                }
            }

            // Priority 2: Finbuckle TenantInfo
            if (_multiTenantContextAccessor?.MultiTenantContext?.TenantInfo?.Identifier != null)
            {
                return _multiTenantContextAccessor.MultiTenantContext.TenantInfo.Identifier;
            }

            // Priority 3: Fallback - matches Audit.TenantId's own default
            return "default_tenant";
        }

        // Seeded via migration (CompanyBrandingId = 1) for "default_tenant", so this only guards
        // an unseeded tenant or a not-yet-migrated dev database - same fallback convention as
        // ApplicationSettingRepository.GetSingletonAsync().
        private static CompanyBranding DefaultBranding(string tenantId) => new()
        {
            CompanyBrandingId = 1,
            TenantId = tenantId,
            CompanyName = "Millennium Information Solution Ltd.",
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

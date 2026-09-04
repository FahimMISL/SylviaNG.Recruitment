using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Admin-managed lookup of special candidate categories (e.g. quota/reserved categories) that a
/// candidate may optionally declare at application time (EP-17/US-127) - referenced by WaiverRule
/// and captured on JobApplication.
/// </summary>
public class SpecialCategory : Audit, ICompanyScoped
{
    public long SpecialCategoryId { get; set; }

    // Multi-tenant: the Company that owns this lookup entry, stamped at creation.
    public long? CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
}

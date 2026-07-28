using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Admin-managed lookup of special candidate categories (e.g. quota/reserved categories) that a
/// candidate may optionally declare at application time (EP-17/US-127) - referenced by WaiverRule
/// and captured on JobApplication.
/// </summary>
public class SpecialCategory : Audit
{
    public long SpecialCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
}

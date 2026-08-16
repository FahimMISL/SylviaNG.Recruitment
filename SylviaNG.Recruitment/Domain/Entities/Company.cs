using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// A tenant organization purchasing/using this recruitment system. SuperAdmin-managed only -
/// Admin/HR users each belong to exactly one Company (see UserAccount.CompanyId) and only ever
/// see their own company's data, enforced by the ICompanyScoped global query filter in
/// ApplicationDBContext. SuperAdmin has no CompanyId and is global across every Company.
/// </summary>
public class Company : Audit
{
    public long CompanyId { get; set; }

    // HRM-integration readiness: the external system's own identifier for this company (nullable,
    // unique when set). Lets a future sync job upsert idempotently by external key instead of
    // matching on Name (which HR can rename) - no sync consumer exists yet, this is just the
    // anchor column so one can be added without a breaking schema change later.
    public string? ExternalId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? LogoFileName { get; set; }
    public string? LogoStoredFileName { get; set; }
    public string? LogoFilePath { get; set; }
    public string? LogoContentType { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Website { get; set; }
    public string? Industry { get; set; }

    // Shadows Audit.Status (int) with a typed enum - same convention as JobPosting.Status/Interview.Status.
    public new CompanyStatusEnum Status { get; set; } = CompanyStatusEnum.Active;

    public ICollection<UserAccount> UserAccounts { get; set; } = new List<UserAccount>();
}

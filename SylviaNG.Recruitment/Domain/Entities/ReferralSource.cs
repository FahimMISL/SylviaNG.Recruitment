using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Admin-managed lookup of how a candidate heard about / was referred to a vacancy (e.g.
/// "Employee Referral", "Job Board"), optionally declared at application time (EP-17/US-127) -
/// referenced by WaiverRule and captured on JobApplication.
/// </summary>
public class ReferralSource : Audit, ICompanyScoped
{
    public long ReferralSourceId { get; set; }

    // Multi-tenant: the Company that owns this lookup entry, stamped at creation.
    public long? CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
}

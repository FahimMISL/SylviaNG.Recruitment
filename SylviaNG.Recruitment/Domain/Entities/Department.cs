using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Admin-managed lookup for job vacancy department options (dynamic dropdown, same pattern as
/// Gender/BloodGroup/etc) - HR/Admin can add/rename/delete values without a code deploy.
/// </summary>
public class Department : Audit
{
    public long DepartmentId { get; set; }
    public string Name { get; set; } = string.Empty;

    // Multi-tenant: null = shared/global department (visible to every company, e.g. the seeded
    // defaults), non-null = a custom department a specific company added for itself. Deliberately
    // NOT ICompanyScoped - that marker's filter has no "visible to everyone" case, which the
    // pre-existing global seed rows need to keep working for every company. See
    // ApplicationDBContext.OnModelCreating for Department's own (lenient) query filter.
    public long? CompanyId { get; set; }
}

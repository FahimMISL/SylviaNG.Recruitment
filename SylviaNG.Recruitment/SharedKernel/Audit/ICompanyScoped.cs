namespace SylviaNG.Recruitment.SharedKernel.Audit;

/// <summary>
/// Marks an entity as strictly company-scoped: ApplicationDBContext.OnModelCreating applies a
/// global EF query filter to every implementor (CompanyId == CurrentCompanyId, or unrestricted
/// when CurrentCompanyId is null - SuperAdmin/system context). Unlike Department (which allows a
/// null CompanyId as a shared/global row), implementors of this interface are expected to always
/// have a real CompanyId once created by the normal invite/create flows.
/// </summary>
public interface ICompanyScoped
{
    long? CompanyId { get; set; }
}

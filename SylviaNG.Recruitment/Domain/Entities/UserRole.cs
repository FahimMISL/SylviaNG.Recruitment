using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class UserRole : Audit
{
    public long UserRoleId { get; set; }
    public long UserId { get; set; }
    public long RoleId { get; set; }
    public long? ScopeBusinessUnitId { get; set; }
    public long? ScopeDepartmentId { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}

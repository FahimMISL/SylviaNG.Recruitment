using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        // RoleId 2 = the HR system role (see RoleConfiguration's SystemRoleNames seed order:
        // Admin=1, HR=2, Candidate=3, SuperAdmin=4). Admin/SuperAdmin don't need rows here - they
        // bypass RequirePermissionAttribute entirely (superuser check). Candidate isn't part of
        // this HR/Admin management surface. Matches exactly the modules/actions this migration
        // wires from [Authorize(Roles="Admin,HR")] to [RequirePermission] - JobPostings/
        // Applications/Interviews/Assessments/Reports - so HR keeps the same real access it had
        // before, just expressed through the permission table instead of a hardcoded role check.
        private const long HrRoleId = 2L;

        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.ToTable("RolePermissions");
            builder.HasKey(rp => new { rp.RoleId, rp.Module, rp.Action });

            builder.HasOne(rp => rp.Role)
                .WithMany(r => r.Permissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasData(HrSeedPermissions());
        }

        private static IEnumerable<RolePermission> HrSeedPermissions()
        {
            (AccessControlModuleEnum Module, PermissionActionEnum[] Actions)[] grants =
            {
                (AccessControlModuleEnum.JobPostings, new[] { PermissionActionEnum.View, PermissionActionEnum.Create, PermissionActionEnum.Edit, PermissionActionEnum.Delete }),
                (AccessControlModuleEnum.Applications, new[] { PermissionActionEnum.View, PermissionActionEnum.Create, PermissionActionEnum.Edit, PermissionActionEnum.Delete }),
                (AccessControlModuleEnum.Interviews, new[] { PermissionActionEnum.View, PermissionActionEnum.Create, PermissionActionEnum.Edit, PermissionActionEnum.Approve }),
                (AccessControlModuleEnum.Assessments, new[] { PermissionActionEnum.View, PermissionActionEnum.Create, PermissionActionEnum.Edit, PermissionActionEnum.Approve }),
                (AccessControlModuleEnum.Reports, new[] { PermissionActionEnum.View }),
            };

            foreach (var (module, actions) in grants)
            {
                foreach (var action in actions)
                {
                    yield return new RolePermission { RoleId = HrRoleId, Module = module, Action = action };
                }
            }
        }
    }
}

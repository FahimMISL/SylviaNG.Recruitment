using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class UserRoleAssignmentConfiguration : IEntityTypeConfiguration<UserRoleAssignment>
    {
        public void Configure(EntityTypeBuilder<UserRoleAssignment> builder)
        {
            builder.ToTable("UserRoleAssignments");
            builder.HasKey(a => new { a.UserAccountId, a.RoleId });

            builder.HasOne(a => a.UserAccount)
                .WithMany(u => u.RoleAssignments)
                .HasForeignKey(a => a.UserAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Role)
                .WithMany(r => r.UserAssignments)
                .HasForeignKey(a => a.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

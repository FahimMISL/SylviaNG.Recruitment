using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class UserAccountConfiguration : IEntityTypeConfiguration<UserAccount>
    {
        public void Configure(EntityTypeBuilder<UserAccount> builder)
        {
            builder.ToTable("UserAccounts");
            builder.HasKey(u => u.UserAccountId);

            builder.Property(u => u.KeycloakUserId).IsRequired().HasMaxLength(64);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
            builder.Property(u => u.FullName).IsRequired().HasMaxLength(200);
            builder.Property(u => u.ExternalEmployeeId).HasMaxLength(200);

            builder.HasIndex(u => u.KeycloakUserId).IsUnique();
            builder.HasIndex(u => u.Email).IsUnique();
            builder.HasIndex(u => u.CompanyId);
            // Per-company, not global - two different companies' external HRM systems can each
            // use their own "employee-1001"-style id independently.
            builder.HasIndex(u => new { u.CompanyId, u.ExternalEmployeeId }).IsUnique();

            // Relationship (Restrict, not Cascade - deleting a Company must go through
            // SetActive/deactivate, never a hard delete cascading into its users) is configured
            // from the Company side - see CompanyConfiguration.
        }
    }
}

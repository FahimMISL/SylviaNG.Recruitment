using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ImpersonationSessionConfiguration : IEntityTypeConfiguration<ImpersonationSession>
    {
        public void Configure(EntityTypeBuilder<ImpersonationSession> builder)
        {
            builder.ToTable("ImpersonationSessions");
            builder.HasKey(s => s.ImpersonationSessionId);

            builder.HasOne(s => s.ActorUserAccount)
                .WithMany()
                .HasForeignKey(s => s.ActorUserAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.TargetUserAccount)
                .WithMany()
                .HasForeignKey(s => s.TargetUserAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

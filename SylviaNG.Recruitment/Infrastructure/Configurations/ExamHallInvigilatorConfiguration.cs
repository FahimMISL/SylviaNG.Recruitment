using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ExamHallInvigilatorConfiguration : IEntityTypeConfiguration<ExamHallInvigilator>
    {
        public void Configure(EntityTypeBuilder<ExamHallInvigilator> builder)
        {
            builder.ToTable("ExamHallInvigilators");
            builder.HasKey(i => new { i.ExamHallId, i.EmployeeId });

            builder.HasOne(i => i.Employee)
                .WithMany()
                .HasForeignKey(i => i.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ExamRoomInvigilatorConfiguration : IEntityTypeConfiguration<ExamRoomInvigilator>
    {
        public void Configure(EntityTypeBuilder<ExamRoomInvigilator> builder)
        {
            builder.ToTable("ExamRoomInvigilators");
            builder.HasKey(i => new { i.ExamRoomId, i.EmployeeId });

            builder.HasOne(i => i.Employee)
                .WithMany()
                .HasForeignKey(i => i.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

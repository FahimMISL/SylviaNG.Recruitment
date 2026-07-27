using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ISeatPlanPdfGeneratorService
    {
        Task<byte[]> Generate(Exam exam, List<ExamEnrollment> enrollments);
    }
}

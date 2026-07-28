using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IAdmitCardPdfGeneratorService
    {
        Task<byte[]> Generate(ExamEnrollment enrollment, Exam exam, JobApplication jobApplication);
    }
}

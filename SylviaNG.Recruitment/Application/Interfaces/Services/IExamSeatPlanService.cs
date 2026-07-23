namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IExamSeatPlanService
    {
        Task GenerateAsync(long examId);
        Task<(byte[] Content, string FileName)> GeneratePdfAsync(long examId);
        Task<(byte[] Content, string FileName)> GenerateExcelAsync(long examId);
        Task<(byte[] Content, string FileName)> GenerateAdmitCardPdfAsync(long examEnrollmentId);
    }
}

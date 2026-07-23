namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IExamSeatPlanService
    {
        Task GenerateAsync(long examId);
        Task<(byte[] Content, string FileName)> GeneratePdfAsync(long examId);
        Task<(byte[] Content, string FileName)> GenerateExcelAsync(long examId);
        Task<(byte[] Content, string FileName)> GenerateAdmitCardPdfAsync(long examEnrollmentId);

        /// <summary>All admit cards for this exam bundled as a single ZIP, one PDF per
        /// enrollment (US-057 AC5).</summary>
        Task<(byte[] Content, string FileName)> GenerateAdmitCardZipAsync(long examId);

        /// <summary>Results (score/pass-fail) for this exam exported to Excel, sorted by score
        /// descending (US-060 AC4).</summary>
        Task<(byte[] Content, string FileName)> GenerateResultsExcelAsync(long examId);
    }
}

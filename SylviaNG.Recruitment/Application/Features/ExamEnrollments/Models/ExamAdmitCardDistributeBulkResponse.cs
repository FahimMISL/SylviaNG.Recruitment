namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models
{
    /// <summary>Summary of a bulk admit-card distribution run (US-057 AC2/AC3).</summary>
    public class ExamAdmitCardDistributeBulkResponse
    {
        public int TotalCount { get; set; }
        public int EmailSentCount { get; set; }
        public int SmsSentCount { get; set; }
    }
}

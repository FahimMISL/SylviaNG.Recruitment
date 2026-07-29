namespace SylviaNG.Recruitment.Application.Features.ExamTaking.Models
{
    /// <summary>US-058 AC6: a reference number is always returned; Score/IsPassed are only
    /// populated when the exam has ShowResultsToCandidate enabled.</summary>
    public class ExamSubmitResultResponse
    {
        public string ReferenceNumber { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public bool ResultsVisible { get; set; }
        public decimal? Score { get; set; }
        public bool? IsPassed { get; set; }
    }
}

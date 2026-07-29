namespace SylviaNG.Recruitment.Application.Features.JobPostings.Models
{
    /// <summary>Real-time eligibility check result for the current candidate against a job posting (US-024 AC2/AC3).</summary>
    public class JobEligibilityResponse
    {
        public bool IsEligible { get; set; }
        public List<string> UnmetRequirements { get; set; } = new();
    }
}

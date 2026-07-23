namespace SylviaNG.Recruitment.Application.Features.JobPostings.Models
{
    /// <summary>HR resolves a detected duplicate group by keeping one primary and dismissing the rest (US-038 AC3).</summary>
    public class JobApplicationDuplicateResolveRequest
    {
        public long JobPostingId { get; set; }
        public long PrimaryJobApplicationId { get; set; }
        public List<long> DuplicateJobApplicationIds { get; set; } = new();
    }
}

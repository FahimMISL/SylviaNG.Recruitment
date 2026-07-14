namespace SylviaNG.Recruitment.Application.Features.JobPostings.Models
{
    /// <summary>Best-effort bulk result (US-035 AC5) - a bad id in a 50-application batch shouldn't fail the other 49.</summary>
    public class JobApplicationBulkStatusUpdateResponse
    {
        public List<long> SucceededIds { get; set; } = new();
        public List<JobApplicationBulkStatusUpdateFailure> Failed { get; set; } = new();
    }

    public class JobApplicationBulkStatusUpdateFailure
    {
        public long JobApplicationId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}

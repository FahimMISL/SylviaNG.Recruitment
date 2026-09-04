namespace SylviaNG.Recruitment.Application.Features.JobPostings.Models
{
    /// <summary>Best-effort bulk result (US-076), same shape as JobApplicationBulkStatusUpdateResponse.</summary>
    public class JobApplicationBulkNotifyResponse
    {
        public List<long> SucceededIds { get; set; } = new();
        public List<JobApplicationBulkNotifyFailure> Failed { get; set; } = new();
    }

    public class JobApplicationBulkNotifyFailure
    {
        public long JobApplicationId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}

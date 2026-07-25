using SylviaNG.Recruitment.Application.Features.JobPostings.Models;

namespace SylviaNG.Recruitment.Application.Features.AutoShortlisting.Models
{
    public class AutoShortlistApplyResponse
    {
        public int TotalProcessed { get; set; }
        public int TotalShortlisted { get; set; }
        public int TotalFailed { get; set; }
        public List<JobApplicationBulkStatusUpdateFailure> Failures { get; set; } = new();
    }
}

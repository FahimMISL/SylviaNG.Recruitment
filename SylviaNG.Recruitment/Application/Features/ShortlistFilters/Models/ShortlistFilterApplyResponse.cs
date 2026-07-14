using SylviaNG.Recruitment.Application.Features.JobPostings.Models;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models
{
    /// <summary>Summary report shown after an apply run (US-044 AC3). TotalProcessed is every
    /// application evaluated against the filter; TotalFailed only counts passing candidates whose
    /// status update itself errored (e.g. a terminal status) - candidates who simply didn't meet
    /// the criteria are neither shortlisted nor failed, they're just left as-is.</summary>
    public class ShortlistFilterApplyResponse
    {
        public int TotalProcessed { get; set; }
        public int TotalShortlisted { get; set; }
        public int TotalFailed { get; set; }
        public List<JobApplicationBulkStatusUpdateFailure> Failures { get; set; } = new();
    }
}

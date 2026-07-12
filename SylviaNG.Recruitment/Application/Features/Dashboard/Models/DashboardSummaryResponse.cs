namespace SylviaNG.Recruitment.Application.Features.Dashboard.Models
{
    public class DashboardSummaryResponse
    {
        public string Role { get; set; } = string.Empty;

        // Admin/HR
        public int? OpenJobPostingsCount { get; set; }
        public int? TotalApplicationsCount { get; set; }
        public int? ActiveHiringPipelinesCount { get; set; }

        // Candidate
        public int? ProfileCompletenessPercentage { get; set; }
    }
}

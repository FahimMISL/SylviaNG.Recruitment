namespace SylviaNG.Recruitment.Application.Features.Dashboard.Models
{
    /// <summary>EP-14 US-105 AC2: a metric with a week-over-week trend delta. Only used for
    /// counters backed by a monotonic/append-only timestamp (e.g. AppliedDate) - there's no
    /// history table to reconstruct a past value for mutable point-in-time state (open vacancy
    /// count, pending approvals, offers pending acceptance), so those stay plain ints with no
    /// trend rather than showing a fabricated/misleading delta.</summary>
    public class DashboardMetric
    {
        public int Count { get; set; }
        public double? ChangePercent { get; set; }
        public string ChangePeriod { get; set; } = "week";
    }

    public class DashboardSummaryResponse
    {
        public string Role { get; set; } = string.Empty;

        // Admin/HR
        public int? OpenJobPostingsCount { get; set; }
        public int? TotalApplicationsCount { get; set; }
        public DashboardMetric? TotalApplicationsTrend { get; set; }
        public Dictionary<string, int>? TotalApplicationsByStatus { get; set; }
        public int? ActiveHiringPipelinesCount { get; set; }
        public int? UpcomingInterviewsCount { get; set; }
        public int? PendingApprovalsCount { get; set; }
        public int? OffersPendingAcceptanceCount { get; set; }
        public List<string>? VisibleWidgetKeys { get; set; }

        // Candidate
        public int? ProfileCompletenessPercentage { get; set; }
    }
}

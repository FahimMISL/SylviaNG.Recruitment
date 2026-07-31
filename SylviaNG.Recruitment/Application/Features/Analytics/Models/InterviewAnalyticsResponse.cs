namespace SylviaNG.Recruitment.Application.Features.Analytics.Models
{
    public class InterviewAnalyticsResponse
    {
        public List<PanelistAnalyticsResponse> Panelists { get; set; } = new();

        /// <summary>AC2: score distribution across every in-scope InterviewEvaluation, not
        /// per-panelist - fixed 5 bands over WeightedScore (0-100).</summary>
        public List<ScoreHistogramBandResponse> ScoreHistogram { get; set; } = new();
    }

    /// <summary>EP-14 US-110 AC1: per-panelist stats. RecommendedCount/NotRecommendedCount/
    /// OnHoldCount only count evaluations that declared a Recommendation - evaluations submitted
    /// before this field existed have none and are excluded from those three counts (but still
    /// counted in InterviewsConducted/AverageScore).</summary>
    public class PanelistAnalyticsResponse
    {
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int InterviewsConducted { get; set; }
        public decimal AverageScore { get; set; }
        public int RecommendedCount { get; set; }
        public int NotRecommendedCount { get; set; }
        public int OnHoldCount { get; set; }
    }

    public class ScoreHistogramBandResponse
    {
        public string Band { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}

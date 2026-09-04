namespace SylviaNG.Recruitment.Application.Features.Analytics.Models
{
    public class CandidateSourceAnalyticsResponse
    {
        public List<CandidateSourceSegmentResponse> Segments { get; set; } = new();
    }

    /// <summary>EP-14 US-108 AC1/AC3: one row per source. SourceLabel is ReferralSource.Name when
    /// declared at apply time, else a fallback label derived from ApplicationSourceEnum - see
    /// AnalyticsReportService.FallbackSourceLabel for the mapping and the feature doc for why (no
    /// single enum covers the named channels in the AC, they live in the ReferralSource lookup).</summary>
    public class CandidateSourceSegmentResponse
    {
        public string SourceLabel { get; set; } = string.Empty;
        public int TotalApplications { get; set; }
        public int ShortlistedCount { get; set; }
        public int HiredCount { get; set; }
        public double ConversionRatePercent { get; set; }
    }
}

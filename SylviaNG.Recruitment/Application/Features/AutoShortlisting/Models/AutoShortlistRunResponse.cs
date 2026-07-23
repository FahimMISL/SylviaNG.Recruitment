namespace SylviaNG.Recruitment.Application.Features.AutoShortlisting.Models
{
    public class AutoShortlistRunResponse
    {
        public long AutoShortlistRunId { get; set; }
        public long JobPostingId { get; set; }
        public string Provider { get; set; } = string.Empty;
        public int CutoffScore { get; set; }
        public DateTime RunAt { get; set; }
        public int TotalApplications { get; set; }
        public int TotalScored { get; set; }
        public int TotalFailed { get; set; }
        public List<AutoShortlistResultResponse> Results { get; set; } = new();
    }
}

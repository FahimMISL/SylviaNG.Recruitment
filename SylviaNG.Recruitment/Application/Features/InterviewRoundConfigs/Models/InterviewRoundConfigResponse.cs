namespace SylviaNG.Recruitment.Application.Features.InterviewRoundConfigs.Models
{
    public class InterviewRoundConfigResponse
    {
        public long InterviewRoundConfigId { get; set; }
        public long JobPostingId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Sequence { get; set; }
        public long? ScorecardId { get; set; }
        public string? ScorecardName { get; set; }
        public List<long> PanelistEmployeeIds { get; set; } = new();
    }
}

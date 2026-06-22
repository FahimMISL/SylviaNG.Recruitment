namespace SylviaNG.Recruitment.Application.Features.InterviewScorecards.Models
{
    public class InterviewScorecardResponse
    {
        public long InterviewScorecardId { get; set; }
        public string ScorecardName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ScoringScale { get; set; }
        public bool IsActive { get; set; }
    }
}

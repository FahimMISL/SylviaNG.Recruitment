namespace SylviaNG.Recruitment.Application.Features.InterviewScorecards.Models
{
    public class InterviewScorecardCreateRequest
    {
        public string ScorecardName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ScoringScale { get; set; }
    }
}

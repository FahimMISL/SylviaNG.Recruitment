namespace SylviaNG.Recruitment.Application.Features.InterviewScorecards.Models
{
    public class InterviewScorecardUpdateRequest
    {
        public string? ScorecardName { get; set; }
        public string? Description { get; set; }
        public string? ScoringScale { get; set; }
        public bool? IsActive { get; set; }
    }
}

namespace SylviaNG.Recruitment.Application.Features.Scorecards.Models
{
    public class ScorecardResponse
    {
        public long ScorecardId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public List<ScorecardCriterionResponse> Criteria { get; set; } = new();
    }
}

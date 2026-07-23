namespace SylviaNG.Recruitment.Application.Features.Scorecards.Models
{
    public class ScorecardCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<ScorecardCriterionRequest> Criteria { get; set; } = new();
    }
}

namespace SylviaNG.Recruitment.Application.Features.Scorecards.Models
{
    public class ScorecardCriterionResponse
    {
        public long ScorecardCriterionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public decimal MaxScore { get; set; }
        public int DisplayOrder { get; set; }
    }
}

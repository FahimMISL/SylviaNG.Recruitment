namespace SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Models
{
    public class ApplicationScreeningResultResponse
    {
        public long ApplicationScreeningResultId { get; set; }
        public long JobApplicationId { get; set; }
        public decimal? RelevanceScore { get; set; }
        public string? MatchedKeywordsJson { get; set; }
        public string? SkillTagsJson { get; set; }
        public string? ExperienceBand { get; set; }
        public string? ScoreExplanation { get; set; }
        public DateTime ScreenedAt { get; set; }
        public bool IsActive { get; set; }
    }
}

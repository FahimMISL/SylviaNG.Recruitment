namespace SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Models
{
    public class InterviewScorecardCriteriaCreateRequest
    {
        public long InterviewScorecardId { get; set; }
        public string CriteriaName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Weight { get; set; }
        public int MaxScore { get; set; }
        public int SortOrder { get; set; }
    }
}

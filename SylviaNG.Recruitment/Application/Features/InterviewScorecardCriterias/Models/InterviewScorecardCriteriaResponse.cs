namespace SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Models
{
    public class InterviewScorecardCriteriaResponse
    {
        public long InterviewScorecardCriteriaId { get; set; }
        public long InterviewScorecardId { get; set; }
        public string CriteriaName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Weight { get; set; }
        public int MaxScore { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }
}

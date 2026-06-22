namespace SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Models
{
    public class InterviewScorecardCriteriaUpdateRequest
    {
        public long? InterviewScorecardId { get; set; }
        public string? CriteriaName { get; set; }
        public string? Description { get; set; }
        public decimal? Weight { get; set; }
        public int? MaxScore { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsActive { get; set; }
    }
}

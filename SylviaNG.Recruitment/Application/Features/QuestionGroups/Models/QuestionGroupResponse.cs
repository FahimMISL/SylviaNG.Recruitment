namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Models
{
    public class QuestionGroupResponse
    {
        public long QuestionGroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? DifficultyLevel { get; set; }
        public bool IsActive { get; set; }
    }
}

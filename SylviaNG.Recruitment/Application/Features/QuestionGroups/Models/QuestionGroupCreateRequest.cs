namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Models
{
    public class QuestionGroupCreateRequest
    {
        public string GroupName { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? DifficultyLevel { get; set; }
    }
}

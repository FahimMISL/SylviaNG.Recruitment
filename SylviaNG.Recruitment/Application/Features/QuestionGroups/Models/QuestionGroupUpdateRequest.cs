namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Models
{
    public class QuestionGroupUpdateRequest
    {
        public string? GroupName { get; set; }
        public string? Category { get; set; }
        public string? DifficultyLevel { get; set; }
        public bool? IsActive { get; set; }
    }
}

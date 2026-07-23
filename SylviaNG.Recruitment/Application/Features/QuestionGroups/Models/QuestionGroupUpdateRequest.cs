namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Models
{
    public class QuestionGroupUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}

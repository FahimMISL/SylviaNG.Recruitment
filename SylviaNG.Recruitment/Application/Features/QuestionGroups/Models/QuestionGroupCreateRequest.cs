namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Models
{
    public class QuestionGroupCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}

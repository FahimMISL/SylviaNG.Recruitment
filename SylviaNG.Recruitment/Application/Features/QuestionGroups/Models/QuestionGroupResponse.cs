namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Models
{
    public class QuestionGroupResponse
    {
        public long QuestionGroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}

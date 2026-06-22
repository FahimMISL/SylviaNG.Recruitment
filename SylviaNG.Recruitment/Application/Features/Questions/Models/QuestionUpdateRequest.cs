using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.Questions.Models
{
    public class QuestionUpdateRequest
    {
        public long? QuestionGroupId { get; set; }
        public QuestionTypeEnum? QuestionType { get; set; }
        public string? QuestionText { get; set; }
        public decimal? Marks { get; set; }
        public int? TimeLimitSeconds { get; set; }
        public string? Explanation { get; set; }
        public bool? IsActive { get; set; }
    }
}

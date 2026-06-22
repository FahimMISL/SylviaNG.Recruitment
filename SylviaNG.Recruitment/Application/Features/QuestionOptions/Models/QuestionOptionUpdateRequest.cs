namespace SylviaNG.Recruitment.Application.Features.QuestionOptions.Models
{
    public class QuestionOptionUpdateRequest
    {
        public long? QuestionId { get; set; }
        public string? OptionText { get; set; }
        public bool? IsCorrect { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsActive { get; set; }
    }
}

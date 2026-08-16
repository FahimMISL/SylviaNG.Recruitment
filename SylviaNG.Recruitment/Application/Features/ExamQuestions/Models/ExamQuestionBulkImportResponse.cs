namespace SylviaNG.Recruitment.Application.Features.ExamQuestions.Models
{
    public class ExamQuestionBulkImportResponse
    {
        public int TotalRows { get; set; }
        public int ImportedCount { get; set; }
        public int FailedCount { get; set; }
        public List<ExamQuestionBulkImportRowError> Errors { get; set; } = new();
    }
}

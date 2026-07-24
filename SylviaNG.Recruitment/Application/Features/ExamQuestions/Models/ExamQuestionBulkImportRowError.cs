namespace SylviaNG.Recruitment.Application.Features.ExamQuestions.Models
{
    public class ExamQuestionBulkImportRowError
    {
        public int RowNumber { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}

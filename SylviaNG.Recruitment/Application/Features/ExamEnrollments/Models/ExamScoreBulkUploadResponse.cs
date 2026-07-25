namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models
{
    public class ExamScoreBulkUploadResponse
    {
        public int TotalRows { get; set; }
        public int UpdatedCount { get; set; }
        public int FailedCount { get; set; }
        public List<ExamScoreBulkUploadRowError> Errors { get; set; } = new();
    }

    public class ExamScoreBulkUploadRowError
    {
        public int RowNumber { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}

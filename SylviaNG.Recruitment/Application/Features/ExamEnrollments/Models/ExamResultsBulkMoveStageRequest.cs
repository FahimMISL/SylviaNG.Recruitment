namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models
{
    public class ExamResultsBulkMoveStageRequest
    {
        public List<long> ExamEnrollmentIds { get; set; } = new();
        public long PipelineStageId { get; set; }
    }
}

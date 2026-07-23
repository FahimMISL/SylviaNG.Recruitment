using MediatR;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Commands.ExamResultsBulkMoveStage
{
    public class ExamResultsBulkMoveStageCommand : IRequest
    {
        public long ExamId { get; set; }
        public List<long> ExamEnrollmentIds { get; set; }
        public long PipelineStageId { get; set; }

        public ExamResultsBulkMoveStageCommand(long examId, List<long> examEnrollmentIds, long pipelineStageId)
        {
            ExamId = examId;
            ExamEnrollmentIds = examEnrollmentIds;
            PipelineStageId = pipelineStageId;
        }
    }
}

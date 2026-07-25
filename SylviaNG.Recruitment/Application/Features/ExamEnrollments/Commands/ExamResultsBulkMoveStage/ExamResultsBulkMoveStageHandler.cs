using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Commands.ExamResultsBulkMoveStage
{
    public class ExamResultsBulkMoveStageHandler : IRequestHandler<ExamResultsBulkMoveStageCommand>
    {
        private readonly IExamEnrollmentService _examEnrollmentService;

        public ExamResultsBulkMoveStageHandler(IExamEnrollmentService examEnrollmentService)
        {
            _examEnrollmentService = examEnrollmentService;
        }

        public async Task Handle(ExamResultsBulkMoveStageCommand command, CancellationToken cancellationToken)
        {
            await _examEnrollmentService.BulkMoveToStageAsync(command.ExamId, command.ExamEnrollmentIds, command.PipelineStageId);
        }
    }
}

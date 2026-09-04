using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Commands.ExamSeatPlanGenerate
{
    public class ExamSeatPlanGenerateHandler : IRequestHandler<ExamSeatPlanGenerateCommand>
    {
        private readonly IExamSeatPlanService _examSeatPlanService;

        public ExamSeatPlanGenerateHandler(IExamSeatPlanService examSeatPlanService)
        {
            _examSeatPlanService = examSeatPlanService;
        }

        public async Task Handle(ExamSeatPlanGenerateCommand command, CancellationToken cancellationToken)
        {
            await _examSeatPlanService.GenerateAsync(command.ExamId);
        }
    }
}

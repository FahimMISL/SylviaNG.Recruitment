using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Commands.ExamSeatPlanUpdate
{
    public class ExamSeatPlanUpdateHandler : IRequestHandler<ExamSeatPlanUpdateCommand, Unit>
    {
        private readonly IExamSeatPlanService _service;

        public ExamSeatPlanUpdateHandler(IExamSeatPlanService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ExamSeatPlanUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.ExamSeatPlanId, command.Request);
            return Unit.Value;
        }
    }
}

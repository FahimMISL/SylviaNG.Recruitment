using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Commands.ExamSeatPlanDelete
{
    public class ExamSeatPlanDeleteHandler : IRequestHandler<ExamSeatPlanDeleteCommand, Unit>
    {
        private readonly IExamSeatPlanService _service;

        public ExamSeatPlanDeleteHandler(IExamSeatPlanService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ExamSeatPlanDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.ExamSeatPlanId);
            return Unit.Value;
        }
    }
}

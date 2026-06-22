using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Commands.ExamSeatPlanCreate
{
    public class ExamSeatPlanCreateHandler : IRequestHandler<ExamSeatPlanCreateCommand, long>
    {
        private readonly IExamSeatPlanService _service;

        public ExamSeatPlanCreateHandler(IExamSeatPlanService service)
        {
            _service = service;
        }

        public async Task<long> Handle(ExamSeatPlanCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

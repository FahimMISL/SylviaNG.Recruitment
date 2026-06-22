using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Commands.InterviewEvaluationUpdate
{
    public class InterviewEvaluationUpdateHandler : IRequestHandler<InterviewEvaluationUpdateCommand, Unit>
    {
        private readonly IInterviewEvaluationService _service;

        public InterviewEvaluationUpdateHandler(IInterviewEvaluationService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(InterviewEvaluationUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.InterviewEvaluationId, command.Request);
            return Unit.Value;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Commands.InterviewEvaluationDelete
{
    public class InterviewEvaluationDeleteHandler : IRequestHandler<InterviewEvaluationDeleteCommand, Unit>
    {
        private readonly IInterviewEvaluationService _service;

        public InterviewEvaluationDeleteHandler(IInterviewEvaluationService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(InterviewEvaluationDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.InterviewEvaluationId);
            return Unit.Value;
        }
    }
}

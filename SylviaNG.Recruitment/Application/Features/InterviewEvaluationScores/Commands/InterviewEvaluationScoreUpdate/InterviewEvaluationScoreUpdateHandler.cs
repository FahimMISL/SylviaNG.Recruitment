using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Commands.InterviewEvaluationScoreUpdate
{
    public class InterviewEvaluationScoreUpdateHandler : IRequestHandler<InterviewEvaluationScoreUpdateCommand, Unit>
    {
        private readonly IInterviewEvaluationScoreService _service;

        public InterviewEvaluationScoreUpdateHandler(IInterviewEvaluationScoreService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(InterviewEvaluationScoreUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.InterviewEvaluationScoreId, command.Request);
            return Unit.Value;
        }
    }
}

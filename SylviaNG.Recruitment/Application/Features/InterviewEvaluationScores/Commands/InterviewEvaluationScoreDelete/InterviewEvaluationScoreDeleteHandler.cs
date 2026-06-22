using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Commands.InterviewEvaluationScoreDelete
{
    public class InterviewEvaluationScoreDeleteHandler : IRequestHandler<InterviewEvaluationScoreDeleteCommand, Unit>
    {
        private readonly IInterviewEvaluationScoreService _service;

        public InterviewEvaluationScoreDeleteHandler(IInterviewEvaluationScoreService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(InterviewEvaluationScoreDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.InterviewEvaluationScoreId);
            return Unit.Value;
        }
    }
}

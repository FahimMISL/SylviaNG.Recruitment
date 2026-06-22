using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Commands.InterviewScorecardCriteriaUpdate
{
    public class InterviewScorecardCriteriaUpdateHandler : IRequestHandler<InterviewScorecardCriteriaUpdateCommand, Unit>
    {
        private readonly IInterviewScorecardCriteriaService _service;

        public InterviewScorecardCriteriaUpdateHandler(IInterviewScorecardCriteriaService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(InterviewScorecardCriteriaUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.InterviewScorecardCriteriaId, command.Request);
            return Unit.Value;
        }
    }
}

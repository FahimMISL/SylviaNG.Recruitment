using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Commands.InterviewScorecardCriteriaDelete
{
    public class InterviewScorecardCriteriaDeleteHandler : IRequestHandler<InterviewScorecardCriteriaDeleteCommand, Unit>
    {
        private readonly IInterviewScorecardCriteriaService _service;

        public InterviewScorecardCriteriaDeleteHandler(IInterviewScorecardCriteriaService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(InterviewScorecardCriteriaDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.InterviewScorecardCriteriaId);
            return Unit.Value;
        }
    }
}

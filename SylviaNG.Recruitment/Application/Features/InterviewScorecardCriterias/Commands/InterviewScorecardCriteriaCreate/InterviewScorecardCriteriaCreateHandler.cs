using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Commands.InterviewScorecardCriteriaCreate
{
    public class InterviewScorecardCriteriaCreateHandler : IRequestHandler<InterviewScorecardCriteriaCreateCommand, long>
    {
        private readonly IInterviewScorecardCriteriaService _service;

        public InterviewScorecardCriteriaCreateHandler(IInterviewScorecardCriteriaService service)
        {
            _service = service;
        }

        public async Task<long> Handle(InterviewScorecardCriteriaCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

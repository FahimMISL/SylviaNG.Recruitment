using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecards.Commands.InterviewScorecardCreate
{
    public class InterviewScorecardCreateHandler : IRequestHandler<InterviewScorecardCreateCommand, long>
    {
        private readonly IInterviewScorecardService _service;

        public InterviewScorecardCreateHandler(IInterviewScorecardService service)
        {
            _service = service;
        }

        public async Task<long> Handle(InterviewScorecardCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

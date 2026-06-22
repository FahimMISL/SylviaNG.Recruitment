using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Candidates.Commands.CandidateCreate
{
    public class CandidateCreateHandler : IRequestHandler<CandidateCreateCommand, long>
    {
        private readonly ICandidateService _service;

        public CandidateCreateHandler(ICandidateService service)
        {
            _service = service;
        }

        public async Task<long> Handle(CandidateCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

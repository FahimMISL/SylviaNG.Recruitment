using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Commands.TalentPoolCandidateCreate
{
    public class TalentPoolCandidateCreateHandler : IRequestHandler<TalentPoolCandidateCreateCommand, long>
    {
        private readonly ITalentPoolCandidateService _service;

        public TalentPoolCandidateCreateHandler(ITalentPoolCandidateService service)
        {
            _service = service;
        }

        public async Task<long> Handle(TalentPoolCandidateCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateTagCreate
{
    public class CandidateTagCreateHandler : IRequestHandler<CandidateTagCreateCommand, long>
    {
        private readonly ICandidateTagService _candidateTagService;

        public CandidateTagCreateHandler(ICandidateTagService candidateTagService)
        {
            _candidateTagService = candidateTagService;
        }

        public async Task<long> Handle(CandidateTagCreateCommand command, CancellationToken cancellationToken)
        {
            return await _candidateTagService.CreateAsync(command.CandidateProfileId, command.Request);
        }
    }
}

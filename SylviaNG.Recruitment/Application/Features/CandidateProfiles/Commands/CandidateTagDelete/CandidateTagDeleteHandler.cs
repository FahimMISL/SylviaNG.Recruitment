using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateTagDelete
{
    public class CandidateTagDeleteHandler : IRequestHandler<CandidateTagDeleteCommand, Unit>
    {
        private readonly ICandidateTagService _candidateTagService;

        public CandidateTagDeleteHandler(ICandidateTagService candidateTagService)
        {
            _candidateTagService = candidateTagService;
        }

        public async Task<Unit> Handle(CandidateTagDeleteCommand command, CancellationToken cancellationToken)
        {
            await _candidateTagService.DeleteAsync(command.CandidateProfileId, command.CandidateTagId);
            return Unit.Value;
        }
    }
}

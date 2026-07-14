using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateProfilePersonalInfoUpdate
{
    public class CandidateProfilePersonalInfoUpdateHandler : IRequestHandler<CandidateProfilePersonalInfoUpdateCommand, Unit>
    {
        private readonly ICandidateProfileService _candidateProfileService;

        public CandidateProfilePersonalInfoUpdateHandler(ICandidateProfileService candidateProfileService)
        {
            _candidateProfileService = candidateProfileService;
        }

        public async Task<Unit> Handle(CandidateProfilePersonalInfoUpdateCommand command, CancellationToken cancellationToken)
        {
            await _candidateProfileService.UpdatePersonalInfoAsync(command.Request);
            return Unit.Value;
        }
    }
}

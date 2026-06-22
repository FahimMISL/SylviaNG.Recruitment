using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Commands.PreBoardingProfileUpdate
{
    public class PreBoardingProfileUpdateHandler : IRequestHandler<PreBoardingProfileUpdateCommand, Unit>
    {
        private readonly IPreBoardingProfileService _service;

        public PreBoardingProfileUpdateHandler(IPreBoardingProfileService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(PreBoardingProfileUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.PreBoardingProfileId, command.Request);
            return Unit.Value;
        }
    }
}

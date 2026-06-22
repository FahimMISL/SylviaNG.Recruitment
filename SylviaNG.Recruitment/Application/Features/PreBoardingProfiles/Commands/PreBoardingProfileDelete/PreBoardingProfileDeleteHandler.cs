using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Commands.PreBoardingProfileDelete
{
    public class PreBoardingProfileDeleteHandler : IRequestHandler<PreBoardingProfileDeleteCommand, Unit>
    {
        private readonly IPreBoardingProfileService _service;

        public PreBoardingProfileDeleteHandler(IPreBoardingProfileService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(PreBoardingProfileDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.PreBoardingProfileId);
            return Unit.Value;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Commands.PreBoardingProfileCreate
{
    public class PreBoardingProfileCreateHandler : IRequestHandler<PreBoardingProfileCreateCommand, long>
    {
        private readonly IPreBoardingProfileService _service;

        public PreBoardingProfileCreateHandler(IPreBoardingProfileService service)
        {
            _service = service;
        }

        public async Task<long> Handle(PreBoardingProfileCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

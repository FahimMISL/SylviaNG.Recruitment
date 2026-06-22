using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Commands.ProfileFieldConfigCreate
{
    public class ProfileFieldConfigCreateHandler : IRequestHandler<ProfileFieldConfigCreateCommand, long>
    {
        private readonly IProfileFieldConfigService _service;

        public ProfileFieldConfigCreateHandler(IProfileFieldConfigService service)
        {
            _service = service;
        }

        public async Task<long> Handle(ProfileFieldConfigCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

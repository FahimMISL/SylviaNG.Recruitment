using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Commands.ProfileFieldConfigUpdate
{
    public class ProfileFieldConfigUpdateHandler : IRequestHandler<ProfileFieldConfigUpdateCommand>
    {
        private readonly IProfileFieldConfigService _service;

        public ProfileFieldConfigUpdateHandler(IProfileFieldConfigService service)
        {
            _service = service;
        }

        public async Task Handle(ProfileFieldConfigUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.Id, command.Request);
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Commands.ProfileFieldConfigDelete
{
    public class ProfileFieldConfigDeleteHandler : IRequestHandler<ProfileFieldConfigDeleteCommand>
    {
        private readonly IProfileFieldConfigService _service;

        public ProfileFieldConfigDeleteHandler(IProfileFieldConfigService service)
        {
            _service = service;
        }

        public async Task Handle(ProfileFieldConfigDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.Id);
        }
    }
}

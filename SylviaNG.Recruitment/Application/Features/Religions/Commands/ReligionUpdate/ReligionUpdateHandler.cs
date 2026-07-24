using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Religions.Commands.ReligionUpdate
{
    public class ReligionUpdateHandler : IRequestHandler<ReligionUpdateCommand, Unit>
    {
        private readonly IReligionService _genderService;

        public ReligionUpdateHandler(IReligionService genderService)
        {
            _genderService = genderService;
        }

        public async Task<Unit> Handle(ReligionUpdateCommand command, CancellationToken cancellationToken)
        {
            await _genderService.UpdateAsync(command.ReligionId, command.Request);
            return Unit.Value;
        }
    }
}

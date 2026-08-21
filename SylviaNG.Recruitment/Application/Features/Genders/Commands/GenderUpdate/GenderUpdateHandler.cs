using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Genders.Commands.GenderUpdate
{
    public class GenderUpdateHandler : IRequestHandler<GenderUpdateCommand, Unit>
    {
        private readonly IGenderService _genderService;

        public GenderUpdateHandler(IGenderService genderService)
        {
            _genderService = genderService;
        }

        public async Task<Unit> Handle(GenderUpdateCommand command, CancellationToken cancellationToken)
        {
            await _genderService.UpdateAsync(command.GenderId, command.Request);
            return Unit.Value;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Religions.Commands.ReligionDelete
{
    public class ReligionDeleteHandler : IRequestHandler<ReligionDeleteCommand, Unit>
    {
        private readonly IReligionService _genderService;

        public ReligionDeleteHandler(IReligionService genderService)
        {
            _genderService = genderService;
        }

        public async Task<Unit> Handle(ReligionDeleteCommand command, CancellationToken cancellationToken)
        {
            await _genderService.DeleteAsync(command.ReligionId);
            return Unit.Value;
        }
    }
}

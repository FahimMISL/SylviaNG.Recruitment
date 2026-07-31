using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Genders.Commands.GenderDelete
{
    public class GenderDeleteHandler : IRequestHandler<GenderDeleteCommand, Unit>
    {
        private readonly IGenderService _genderService;

        public GenderDeleteHandler(IGenderService genderService)
        {
            _genderService = genderService;
        }

        public async Task<Unit> Handle(GenderDeleteCommand command, CancellationToken cancellationToken)
        {
            await _genderService.DeleteAsync(command.GenderId);
            return Unit.Value;
        }
    }
}

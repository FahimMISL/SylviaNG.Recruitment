using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Genders.Commands.GenderCreate
{
    public class GenderCreateHandler : IRequestHandler<GenderCreateCommand, long>
    {
        private readonly IGenderService _genderService;

        public GenderCreateHandler(IGenderService genderService)
        {
            _genderService = genderService;
        }

        public async Task<long> Handle(GenderCreateCommand command, CancellationToken cancellationToken)
        {
            return await _genderService.CreateAsync(command.Request);
        }
    }
}

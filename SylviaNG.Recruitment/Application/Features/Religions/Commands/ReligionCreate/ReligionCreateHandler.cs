using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Religions.Commands.ReligionCreate
{
    public class ReligionCreateHandler : IRequestHandler<ReligionCreateCommand, long>
    {
        private readonly IReligionService _genderService;

        public ReligionCreateHandler(IReligionService genderService)
        {
            _genderService = genderService;
        }

        public async Task<long> Handle(ReligionCreateCommand command, CancellationToken cancellationToken)
        {
            return await _genderService.CreateAsync(command.Request);
        }
    }
}

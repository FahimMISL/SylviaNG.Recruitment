using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.MaritalStatuses.Commands.MaritalStatusCreate
{
    public class MaritalStatusCreateHandler : IRequestHandler<MaritalStatusCreateCommand, long>
    {
        private readonly IMaritalStatusService _genderService;

        public MaritalStatusCreateHandler(IMaritalStatusService genderService)
        {
            _genderService = genderService;
        }

        public async Task<long> Handle(MaritalStatusCreateCommand command, CancellationToken cancellationToken)
        {
            return await _genderService.CreateAsync(command.Request);
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.MaritalStatuses.Commands.MaritalStatusDelete
{
    public class MaritalStatusDeleteHandler : IRequestHandler<MaritalStatusDeleteCommand, Unit>
    {
        private readonly IMaritalStatusService _genderService;

        public MaritalStatusDeleteHandler(IMaritalStatusService genderService)
        {
            _genderService = genderService;
        }

        public async Task<Unit> Handle(MaritalStatusDeleteCommand command, CancellationToken cancellationToken)
        {
            await _genderService.DeleteAsync(command.MaritalStatusId);
            return Unit.Value;
        }
    }
}

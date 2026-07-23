using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.MaritalStatuses.Commands.MaritalStatusUpdate
{
    public class MaritalStatusUpdateHandler : IRequestHandler<MaritalStatusUpdateCommand, Unit>
    {
        private readonly IMaritalStatusService _genderService;

        public MaritalStatusUpdateHandler(IMaritalStatusService genderService)
        {
            _genderService = genderService;
        }

        public async Task<Unit> Handle(MaritalStatusUpdateCommand command, CancellationToken cancellationToken)
        {
            await _genderService.UpdateAsync(command.MaritalStatusId, command.Request);
            return Unit.Value;
        }
    }
}

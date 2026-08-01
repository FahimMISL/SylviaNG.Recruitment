using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ReferralSources.Commands.ReferralSourceUpdate
{
    public class ReferralSourceUpdateHandler : IRequestHandler<ReferralSourceUpdateCommand, Unit>
    {
        private readonly IReferralSourceService _referralSourceService;

        public ReferralSourceUpdateHandler(IReferralSourceService referralSourceService)
        {
            _referralSourceService = referralSourceService;
        }

        public async Task<Unit> Handle(ReferralSourceUpdateCommand command, CancellationToken cancellationToken)
        {
            await _referralSourceService.UpdateAsync(command.ReferralSourceId, command.Request);
            return Unit.Value;
        }
    }
}

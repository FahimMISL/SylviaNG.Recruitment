using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ReferralSources.Commands.ReferralSourceDelete
{
    public class ReferralSourceDeleteHandler : IRequestHandler<ReferralSourceDeleteCommand, Unit>
    {
        private readonly IReferralSourceService _referralSourceService;

        public ReferralSourceDeleteHandler(IReferralSourceService referralSourceService)
        {
            _referralSourceService = referralSourceService;
        }

        public async Task<Unit> Handle(ReferralSourceDeleteCommand command, CancellationToken cancellationToken)
        {
            await _referralSourceService.DeleteAsync(command.ReferralSourceId);
            return Unit.Value;
        }
    }
}

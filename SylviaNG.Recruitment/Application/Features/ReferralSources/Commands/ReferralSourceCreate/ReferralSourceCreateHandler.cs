using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ReferralSources.Commands.ReferralSourceCreate
{
    public class ReferralSourceCreateHandler : IRequestHandler<ReferralSourceCreateCommand, long>
    {
        private readonly IReferralSourceService _referralSourceService;

        public ReferralSourceCreateHandler(IReferralSourceService referralSourceService)
        {
            _referralSourceService = referralSourceService;
        }

        public async Task<long> Handle(ReferralSourceCreateCommand command, CancellationToken cancellationToken)
        {
            return await _referralSourceService.CreateAsync(command.Request);
        }
    }
}

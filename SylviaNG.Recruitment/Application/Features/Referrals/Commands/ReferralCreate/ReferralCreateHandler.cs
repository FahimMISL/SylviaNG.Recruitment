using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Referrals.Commands.ReferralCreate
{
    public class ReferralCreateHandler : IRequestHandler<ReferralCreateCommand, long>
    {
        private readonly IReferralService _service;

        public ReferralCreateHandler(IReferralService service)
        {
            _service = service;
        }

        public async Task<long> Handle(ReferralCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

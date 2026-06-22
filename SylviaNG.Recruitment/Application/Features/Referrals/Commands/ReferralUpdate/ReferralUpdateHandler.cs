using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Referrals.Commands.ReferralUpdate
{
    public class ReferralUpdateHandler : IRequestHandler<ReferralUpdateCommand, Unit>
    {
        private readonly IReferralService _service;

        public ReferralUpdateHandler(IReferralService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ReferralUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.ReferralId, command.Request);
            return Unit.Value;
        }
    }
}

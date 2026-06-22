using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Referrals.Commands.ReferralDelete
{
    public class ReferralDeleteHandler : IRequestHandler<ReferralDeleteCommand, Unit>
    {
        private readonly IReferralService _service;

        public ReferralDeleteHandler(IReferralService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ReferralDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.ReferralId);
            return Unit.Value;
        }
    }
}

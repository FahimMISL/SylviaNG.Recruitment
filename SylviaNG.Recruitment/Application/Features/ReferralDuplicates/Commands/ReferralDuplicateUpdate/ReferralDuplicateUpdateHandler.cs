using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Commands.ReferralDuplicateUpdate
{
    public class ReferralDuplicateUpdateHandler : IRequestHandler<ReferralDuplicateUpdateCommand, Unit>
    {
        private readonly IReferralDuplicateService _service;

        public ReferralDuplicateUpdateHandler(IReferralDuplicateService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ReferralDuplicateUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.ReferralDuplicateId, command.Request);
            return Unit.Value;
        }
    }
}

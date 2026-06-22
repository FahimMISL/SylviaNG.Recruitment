using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Commands.ReferralDuplicateDelete
{
    public class ReferralDuplicateDeleteHandler : IRequestHandler<ReferralDuplicateDeleteCommand, Unit>
    {
        private readonly IReferralDuplicateService _service;

        public ReferralDuplicateDeleteHandler(IReferralDuplicateService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ReferralDuplicateDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.ReferralDuplicateId);
            return Unit.Value;
        }
    }
}

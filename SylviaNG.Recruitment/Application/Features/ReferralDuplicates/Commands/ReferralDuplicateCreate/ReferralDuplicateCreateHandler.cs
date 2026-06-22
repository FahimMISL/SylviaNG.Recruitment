using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Commands.ReferralDuplicateCreate
{
    public class ReferralDuplicateCreateHandler : IRequestHandler<ReferralDuplicateCreateCommand, long>
    {
        private readonly IReferralDuplicateService _service;

        public ReferralDuplicateCreateHandler(IReferralDuplicateService service)
        {
            _service = service;
        }

        public async Task<long> Handle(ReferralDuplicateCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

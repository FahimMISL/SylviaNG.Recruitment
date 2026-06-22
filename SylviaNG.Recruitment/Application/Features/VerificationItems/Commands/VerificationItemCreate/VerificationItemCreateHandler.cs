using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.VerificationItems.Commands.VerificationItemCreate
{
    public class VerificationItemCreateHandler : IRequestHandler<VerificationItemCreateCommand, long>
    {
        private readonly IVerificationItemService _service;

        public VerificationItemCreateHandler(IVerificationItemService service)
        {
            _service = service;
        }

        public async Task<long> Handle(VerificationItemCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

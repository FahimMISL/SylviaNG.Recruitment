using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.VerificationItems.Commands.VerificationItemUpdate
{
    public class VerificationItemUpdateHandler : IRequestHandler<VerificationItemUpdateCommand, Unit>
    {
        private readonly IVerificationItemService _service;

        public VerificationItemUpdateHandler(IVerificationItemService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(VerificationItemUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.VerificationItemId, command.Request);
            return Unit.Value;
        }
    }
}

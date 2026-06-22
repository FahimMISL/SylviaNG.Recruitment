using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.VerificationItems.Commands.VerificationItemDelete
{
    public class VerificationItemDeleteHandler : IRequestHandler<VerificationItemDeleteCommand, Unit>
    {
        private readonly IVerificationItemService _service;

        public VerificationItemDeleteHandler(IVerificationItemService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(VerificationItemDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.VerificationItemId);
            return Unit.Value;
        }
    }
}

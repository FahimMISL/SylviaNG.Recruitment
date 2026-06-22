using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Commands.DocumentAcceptanceUpdate
{
    public class DocumentAcceptanceUpdateHandler : IRequestHandler<DocumentAcceptanceUpdateCommand, Unit>
    {
        private readonly IDocumentAcceptanceService _service;

        public DocumentAcceptanceUpdateHandler(IDocumentAcceptanceService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(DocumentAcceptanceUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.DocumentAcceptanceId, command.Request);
            return Unit.Value;
        }
    }
}

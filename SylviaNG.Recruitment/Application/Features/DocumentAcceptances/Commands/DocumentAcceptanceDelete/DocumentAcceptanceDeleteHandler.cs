using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Commands.DocumentAcceptanceDelete
{
    public class DocumentAcceptanceDeleteHandler : IRequestHandler<DocumentAcceptanceDeleteCommand, Unit>
    {
        private readonly IDocumentAcceptanceService _service;

        public DocumentAcceptanceDeleteHandler(IDocumentAcceptanceService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(DocumentAcceptanceDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.DocumentAcceptanceId);
            return Unit.Value;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Commands.DocumentAcceptanceCreate
{
    public class DocumentAcceptanceCreateHandler : IRequestHandler<DocumentAcceptanceCreateCommand, long>
    {
        private readonly IDocumentAcceptanceService _service;

        public DocumentAcceptanceCreateHandler(IDocumentAcceptanceService service)
        {
            _service = service;
        }

        public async Task<long> Handle(DocumentAcceptanceCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

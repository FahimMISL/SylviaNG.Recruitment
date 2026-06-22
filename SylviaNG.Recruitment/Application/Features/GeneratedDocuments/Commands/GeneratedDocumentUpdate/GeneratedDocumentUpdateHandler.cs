using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Commands.GeneratedDocumentUpdate
{
    public class GeneratedDocumentUpdateHandler : IRequestHandler<GeneratedDocumentUpdateCommand, Unit>
    {
        private readonly IGeneratedDocumentService _service;

        public GeneratedDocumentUpdateHandler(IGeneratedDocumentService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(GeneratedDocumentUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.GeneratedDocumentId, command.Request);
            return Unit.Value;
        }
    }
}

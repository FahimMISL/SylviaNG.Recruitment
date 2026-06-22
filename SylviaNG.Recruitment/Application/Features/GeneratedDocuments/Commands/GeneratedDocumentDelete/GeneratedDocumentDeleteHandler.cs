using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Commands.GeneratedDocumentDelete
{
    public class GeneratedDocumentDeleteHandler : IRequestHandler<GeneratedDocumentDeleteCommand, Unit>
    {
        private readonly IGeneratedDocumentService _service;

        public GeneratedDocumentDeleteHandler(IGeneratedDocumentService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(GeneratedDocumentDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.GeneratedDocumentId);
            return Unit.Value;
        }
    }
}

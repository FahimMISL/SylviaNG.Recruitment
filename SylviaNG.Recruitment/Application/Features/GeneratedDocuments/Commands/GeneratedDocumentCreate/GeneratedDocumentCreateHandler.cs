using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Commands.GeneratedDocumentCreate
{
    public class GeneratedDocumentCreateHandler : IRequestHandler<GeneratedDocumentCreateCommand, long>
    {
        private readonly IGeneratedDocumentService _service;

        public GeneratedDocumentCreateHandler(IGeneratedDocumentService service)
        {
            _service = service;
        }

        public async Task<long> Handle(GeneratedDocumentCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

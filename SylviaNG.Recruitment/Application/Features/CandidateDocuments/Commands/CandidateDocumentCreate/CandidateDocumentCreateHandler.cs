using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateDocuments.Commands.CandidateDocumentCreate
{
    public class CandidateDocumentCreateHandler : IRequestHandler<CandidateDocumentCreateCommand, long>
    {
        private readonly ICandidateDocumentService _service;

        public CandidateDocumentCreateHandler(ICandidateDocumentService service)
        {
            _service = service;
        }

        public async Task<long> Handle(CandidateDocumentCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateDocuments.Commands.CandidateDocumentUpdate
{
    public class CandidateDocumentUpdateHandler : IRequestHandler<CandidateDocumentUpdateCommand, Unit>
    {
        private readonly ICandidateDocumentService _service;

        public CandidateDocumentUpdateHandler(ICandidateDocumentService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(CandidateDocumentUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.CandidateDocumentId, command.Request);
            return Unit.Value;
        }
    }
}

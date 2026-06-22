using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateDocuments.Commands.CandidateDocumentDelete
{
    public class CandidateDocumentDeleteHandler : IRequestHandler<CandidateDocumentDeleteCommand, Unit>
    {
        private readonly ICandidateDocumentService _service;

        public CandidateDocumentDeleteHandler(ICandidateDocumentService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(CandidateDocumentDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.CandidateDocumentId);
            return Unit.Value;
        }
    }
}

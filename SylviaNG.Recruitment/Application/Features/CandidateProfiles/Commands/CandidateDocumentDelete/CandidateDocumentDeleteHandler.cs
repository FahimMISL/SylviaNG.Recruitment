using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateDocumentDelete
{
    public class CandidateDocumentDeleteHandler : IRequestHandler<CandidateDocumentDeleteCommand, Unit>
    {
        private readonly ICandidateDocumentService _candidateDocumentService;

        public CandidateDocumentDeleteHandler(ICandidateDocumentService candidateDocumentService)
        {
            _candidateDocumentService = candidateDocumentService;
        }

        public async Task<Unit> Handle(CandidateDocumentDeleteCommand command, CancellationToken cancellationToken)
        {
            await _candidateDocumentService.DeleteAsync(command.CandidateDocumentId);
            return Unit.Value;
        }
    }
}

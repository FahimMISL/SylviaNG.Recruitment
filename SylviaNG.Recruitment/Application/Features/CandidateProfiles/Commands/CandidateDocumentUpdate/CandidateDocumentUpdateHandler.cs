using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateDocumentUpdate
{
    public class CandidateDocumentUpdateHandler : IRequestHandler<CandidateDocumentUpdateCommand, CandidateDocumentResponse>
    {
        private readonly ICandidateDocumentService _candidateDocumentService;

        public CandidateDocumentUpdateHandler(ICandidateDocumentService candidateDocumentService)
        {
            _candidateDocumentService = candidateDocumentService;
        }

        public async Task<CandidateDocumentResponse> Handle(CandidateDocumentUpdateCommand command, CancellationToken cancellationToken)
        {
            return await _candidateDocumentService.UpdateAsync(command.CandidateDocumentId, command.Request);
        }
    }
}

using MediatR;
using Microsoft.Extensions.Logging;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateResumeParse
{
    public class CandidateResumeParseHandler : IRequestHandler<CandidateResumeParseCommand, CandidateResumeParseResponse>
    {
        private readonly IResumeParsingService _resumeParsingService;
        private readonly ICandidateDocumentService _candidateDocumentService;
        private readonly ILogger<CandidateResumeParseHandler> _logger;

        public CandidateResumeParseHandler(
            IResumeParsingService resumeParsingService,
            ICandidateDocumentService candidateDocumentService,
            ILogger<CandidateResumeParseHandler> logger)
        {
            _resumeParsingService = resumeParsingService;
            _candidateDocumentService = candidateDocumentService;
            _logger = logger;
        }

        public async Task<CandidateResumeParseResponse> Handle(CandidateResumeParseCommand command, CancellationToken cancellationToken)
        {
            var result = await _resumeParsingService.ParseAsync(command.File);

            // Saves the uploaded file itself as a Resume document in the same request, so the
            // candidate isn't asked to upload the same file a second time via the Documents
            // section. Best-effort: a save failure must not take down an otherwise-successful
            // parse, so it's isolated here rather than left to bubble out of Handle.
            try
            {
                await _candidateDocumentService.UploadAsync(new CandidateDocumentUploadRequest
                {
                    DocumentType = CandidateDocumentTypeEnum.Resume,
                    File = command.File
                });
                result.ResumeDocumentSaved = true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to save uploaded resume as a CandidateDocument; parse result is still returned.");
            }

            return result;
        }
    }
}

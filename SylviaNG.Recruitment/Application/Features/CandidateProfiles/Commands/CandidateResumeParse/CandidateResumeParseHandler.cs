using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateResumeParse
{
    public class CandidateResumeParseHandler : IRequestHandler<CandidateResumeParseCommand, CandidateResumeParseResponse>
    {
        private readonly IResumeParsingService _resumeParsingService;

        public CandidateResumeParseHandler(IResumeParsingService resumeParsingService)
        {
            _resumeParsingService = resumeParsingService;
        }

        public async Task<CandidateResumeParseResponse> Handle(CandidateResumeParseCommand command, CancellationToken cancellationToken)
        {
            return await _resumeParsingService.ParseAsync(command.File);
        }
    }
}

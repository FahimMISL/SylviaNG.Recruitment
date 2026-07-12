using MediatR;
using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateResumeParse
{
    public class CandidateResumeParseCommand : IRequest<CandidateResumeParseResponse>
    {
        public IFormFile File { get; set; }

        public CandidateResumeParseCommand(IFormFile file)
        {
            File = file;
        }
    }
}

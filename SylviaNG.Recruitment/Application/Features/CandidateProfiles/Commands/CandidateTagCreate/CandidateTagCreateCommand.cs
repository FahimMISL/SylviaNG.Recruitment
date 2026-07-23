using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateTagCreate
{
    public class CandidateTagCreateCommand : IRequest<long>
    {
        public long CandidateProfileId { get; set; }
        public CandidateTagCreateRequest Request { get; set; }

        public CandidateTagCreateCommand(long candidateProfileId, CandidateTagCreateRequest request)
        {
            CandidateProfileId = candidateProfileId;
            Request = request;
        }
    }
}

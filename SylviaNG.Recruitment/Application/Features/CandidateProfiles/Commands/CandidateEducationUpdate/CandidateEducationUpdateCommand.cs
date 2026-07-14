using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateEducationUpdate
{
    public class CandidateEducationUpdateCommand : IRequest<Unit>
    {
        public long CandidateEducationId { get; set; }
        public CandidateEducationUpdateRequest Request { get; set; }

        public CandidateEducationUpdateCommand(long candidateEducationId, CandidateEducationUpdateRequest request)
        {
            CandidateEducationId = candidateEducationId;
            Request = request;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Features.Candidates.Models;

namespace SylviaNG.Recruitment.Application.Features.Candidates.Commands.CandidateUpdate
{
    public class CandidateUpdateCommand : IRequest<Unit>
    {
        public long CandidateId { get; set; }
        public CandidateUpdateRequest Request { get; set; }

        public CandidateUpdateCommand(long candidateId, CandidateUpdateRequest request)
        {
            CandidateId = candidateId;
            Request = request;
        }
    }
}

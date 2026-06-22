using MediatR;
using SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Models;

namespace SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Commands.TalentPoolCandidateUpdate
{
    public class TalentPoolCandidateUpdateCommand : IRequest<Unit>
    {
        public long TalentPoolCandidateId { get; set; }
        public TalentPoolCandidateUpdateRequest Request { get; set; }

        public TalentPoolCandidateUpdateCommand(long talentPoolCandidateId, TalentPoolCandidateUpdateRequest request)
        {
            TalentPoolCandidateId = talentPoolCandidateId;
            Request = request;
        }
    }
}

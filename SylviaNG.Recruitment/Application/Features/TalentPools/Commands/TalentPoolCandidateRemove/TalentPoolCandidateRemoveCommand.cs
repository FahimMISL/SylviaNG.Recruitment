using MediatR;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolCandidateRemove
{
    public class TalentPoolCandidateRemoveCommand : IRequest
    {
        public long TalentPoolId { get; set; }
        public long CandidateProfileId { get; set; }

        public TalentPoolCandidateRemoveCommand(long talentPoolId, long candidateProfileId)
        {
            TalentPoolId = talentPoolId;
            CandidateProfileId = candidateProfileId;
        }
    }
}

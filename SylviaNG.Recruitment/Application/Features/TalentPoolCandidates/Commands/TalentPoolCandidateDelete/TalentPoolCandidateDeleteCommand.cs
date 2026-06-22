using MediatR;

namespace SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Commands.TalentPoolCandidateDelete
{
    public class TalentPoolCandidateDeleteCommand : IRequest<Unit>
    {
        public long TalentPoolCandidateId { get; set; }

        public TalentPoolCandidateDeleteCommand(long talentPoolCandidateId)
        {
            TalentPoolCandidateId = talentPoolCandidateId;
        }
    }
}

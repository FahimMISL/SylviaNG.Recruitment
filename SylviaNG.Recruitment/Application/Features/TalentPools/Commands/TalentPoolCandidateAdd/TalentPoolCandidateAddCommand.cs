using MediatR;
using SylviaNG.Recruitment.Application.Features.TalentPools.Models;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolCandidateAdd
{
    public class TalentPoolCandidateAddCommand : IRequest<TalentPoolCandidateAddResponse>
    {
        public long TalentPoolId { get; set; }
        public TalentPoolCandidateAddRequest Request { get; set; }

        public TalentPoolCandidateAddCommand(long talentPoolId, TalentPoolCandidateAddRequest request)
        {
            TalentPoolId = talentPoolId;
            Request = request;
        }
    }
}

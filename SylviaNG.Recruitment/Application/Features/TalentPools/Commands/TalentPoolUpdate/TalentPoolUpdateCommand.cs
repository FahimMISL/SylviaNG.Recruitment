using MediatR;
using SylviaNG.Recruitment.Application.Features.TalentPools.Models;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolUpdate
{
    public class TalentPoolUpdateCommand : IRequest<Unit>
    {
        public long TalentPoolId { get; set; }
        public TalentPoolUpdateRequest Request { get; set; }

        public TalentPoolUpdateCommand(long talentPoolId, TalentPoolUpdateRequest request)
        {
            TalentPoolId = talentPoolId;
            Request = request;
        }
    }
}

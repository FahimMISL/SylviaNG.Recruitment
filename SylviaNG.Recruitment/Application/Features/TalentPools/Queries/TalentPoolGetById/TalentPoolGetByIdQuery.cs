using MediatR;
using SylviaNG.Recruitment.Application.Features.TalentPools.Models;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Queries.TalentPoolGetById
{
    public class TalentPoolGetByIdQuery : IRequest<TalentPoolDetailResponse>
    {
        public long TalentPoolId { get; set; }

        public TalentPoolGetByIdQuery(long talentPoolId)
        {
            TalentPoolId = talentPoolId;
        }
    }
}

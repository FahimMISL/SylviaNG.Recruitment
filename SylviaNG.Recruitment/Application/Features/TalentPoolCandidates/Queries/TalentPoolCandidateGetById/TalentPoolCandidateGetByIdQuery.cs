using MediatR;
using SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Models;

namespace SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Queries.TalentPoolCandidateGetById
{
    public class TalentPoolCandidateGetByIdQuery : IRequest<TalentPoolCandidateResponse>
    {
        public long TalentPoolCandidateId { get; set; }

        public TalentPoolCandidateGetByIdQuery(long talentPoolCandidateId)
        {
            TalentPoolCandidateId = talentPoolCandidateId;
        }
    }
}

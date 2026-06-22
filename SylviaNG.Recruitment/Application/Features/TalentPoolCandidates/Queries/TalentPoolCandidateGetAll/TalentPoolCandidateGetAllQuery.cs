using MediatR;
using SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Models;

namespace SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Queries.TalentPoolCandidateGetAll
{
    public class TalentPoolCandidateGetAllQuery : IRequest<List<TalentPoolCandidateResponse>>
    {
    }
}

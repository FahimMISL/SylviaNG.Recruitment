using MediatR;
using SylviaNG.Recruitment.Application.Features.TalentPools.Models;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Queries.TalentPoolLookup
{
    public class TalentPoolLookupQuery : IRequest<List<TalentPoolLookupResponse>>
    {
    }
}

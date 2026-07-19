using MediatR;
using SylviaNG.Recruitment.Application.Features.TalentPools.Models;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Queries.TalentPoolGetAll
{
    public class TalentPoolGetAllQuery : IRequest<List<TalentPoolResponse>>
    {
    }
}

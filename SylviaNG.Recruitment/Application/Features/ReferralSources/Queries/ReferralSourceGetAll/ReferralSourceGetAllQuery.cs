using MediatR;
using SylviaNG.Recruitment.Application.Features.ReferralSources.Models;

namespace SylviaNG.Recruitment.Application.Features.ReferralSources.Queries.ReferralSourceGetAll
{
    public class ReferralSourceGetAllQuery : IRequest<List<ReferralSourceResponse>>
    {
    }
}

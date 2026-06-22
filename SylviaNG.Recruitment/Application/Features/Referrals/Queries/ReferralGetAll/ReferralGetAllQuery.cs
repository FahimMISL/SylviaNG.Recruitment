using MediatR;
using SylviaNG.Recruitment.Application.Features.Referrals.Models;

namespace SylviaNG.Recruitment.Application.Features.Referrals.Queries.ReferralGetAll
{
    public class ReferralGetAllQuery : IRequest<List<ReferralResponse>>
    {
    }
}

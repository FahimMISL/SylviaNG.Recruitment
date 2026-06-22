using MediatR;
using SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Models;

namespace SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Queries.ReferralDuplicateGetAll
{
    public class ReferralDuplicateGetAllQuery : IRequest<List<ReferralDuplicateResponse>>
    {
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Features.WaiverRules.Models;

namespace SylviaNG.Recruitment.Application.Features.WaiverRules.Queries.WaiverRuleGetAll
{
    public class WaiverRuleGetAllQuery : IRequest<List<WaiverRuleResponse>>
    {
    }
}

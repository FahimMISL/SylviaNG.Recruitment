using MediatR;
using SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Models;

namespace SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Queries.RecruitmentAgencyGetAll
{
    public class RecruitmentAgencyGetAllQuery : IRequest<List<RecruitmentAgencyResponse>>
    {
    }
}

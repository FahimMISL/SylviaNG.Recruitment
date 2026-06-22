using MediatR;
using SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Models;

namespace SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Queries.RecruitmentAgencyGetById
{
    public class RecruitmentAgencyGetByIdQuery : IRequest<RecruitmentAgencyResponse>
    {
        public long RecruitmentAgencyId { get; set; }

        public RecruitmentAgencyGetByIdQuery(long recruitmentAgencyId)
        {
            RecruitmentAgencyId = recruitmentAgencyId;
        }
    }
}

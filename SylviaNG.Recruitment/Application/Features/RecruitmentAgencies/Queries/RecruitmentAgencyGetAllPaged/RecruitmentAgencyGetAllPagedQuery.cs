using MediatR;
using SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Queries.RecruitmentAgencyGetAllPaged
{
    public class RecruitmentAgencyGetAllPagedQuery : IRequest<PagedResult<RecruitmentAgencyResponse>>
    {
        public PagedRequest Request { get; set; }

        public RecruitmentAgencyGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

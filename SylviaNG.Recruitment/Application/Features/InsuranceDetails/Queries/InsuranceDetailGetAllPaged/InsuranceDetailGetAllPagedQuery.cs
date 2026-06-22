using MediatR;
using SylviaNG.Recruitment.Application.Features.InsuranceDetails.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.InsuranceDetails.Queries.InsuranceDetailGetAllPaged
{
    public class InsuranceDetailGetAllPagedQuery : IRequest<PagedResult<InsuranceDetailResponse>>
    {
        public PagedRequest Request { get; set; }

        public InsuranceDetailGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Queries.ReferralDuplicateGetAllPaged
{
    public class ReferralDuplicateGetAllPagedQuery : IRequest<PagedResult<ReferralDuplicateResponse>>
    {
        public PagedRequest Request { get; set; }

        public ReferralDuplicateGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

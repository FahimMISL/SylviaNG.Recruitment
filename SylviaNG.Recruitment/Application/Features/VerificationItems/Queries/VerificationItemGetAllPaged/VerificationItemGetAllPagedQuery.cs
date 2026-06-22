using MediatR;
using SylviaNG.Recruitment.Application.Features.VerificationItems.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.VerificationItems.Queries.VerificationItemGetAllPaged
{
    public class VerificationItemGetAllPagedQuery : IRequest<PagedResult<VerificationItemResponse>>
    {
        public PagedRequest Request { get; set; }

        public VerificationItemGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

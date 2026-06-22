using MediatR;
using SylviaNG.Recruitment.Application.Features.Nominees.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.Nominees.Queries.NomineeGetAllPaged
{
    public class NomineeGetAllPagedQuery : IRequest<PagedResult<NomineeResponse>>
    {
        public PagedRequest Request { get; set; }

        public NomineeGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

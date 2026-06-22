using MediatR;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Queries.JoiningBookletGetAllPaged
{
    public class JoiningBookletGetAllPagedQuery : IRequest<PagedResult<JoiningBookletResponse>>
    {
        public PagedRequest Request { get; set; }

        public JoiningBookletGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

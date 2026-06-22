using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ExamHalls.Queries.ExamHallGetAllPaged
{
    public class ExamHallGetAllPagedQuery : IRequest<PagedResult<ExamHallResponse>>
    {
        public PagedRequest Request { get; set; }

        public ExamHallGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

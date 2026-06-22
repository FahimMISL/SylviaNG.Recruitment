using MediatR;
using SylviaNG.Recruitment.Application.Features.Exams.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.Exams.Queries.ExamGetAllPaged
{
    public class ExamGetAllPagedQuery : IRequest<PagedResult<ExamResponse>>
    {
        public PagedRequest Request { get; set; }

        public ExamGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

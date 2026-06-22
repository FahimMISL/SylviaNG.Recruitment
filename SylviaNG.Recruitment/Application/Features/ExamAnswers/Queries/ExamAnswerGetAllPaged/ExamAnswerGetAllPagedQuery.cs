using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamAnswers.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ExamAnswers.Queries.ExamAnswerGetAllPaged
{
    public class ExamAnswerGetAllPagedQuery : IRequest<PagedResult<ExamAnswerResponse>>
    {
        public PagedRequest Request { get; set; }

        public ExamAnswerGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

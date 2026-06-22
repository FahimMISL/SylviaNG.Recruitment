using MediatR;
using SylviaNG.Recruitment.Application.Features.Questions.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.Questions.Queries.QuestionGetAllPaged
{
    public class QuestionGetAllPagedQuery : IRequest<PagedResult<QuestionResponse>>
    {
        public PagedRequest Request { get; set; }

        public QuestionGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

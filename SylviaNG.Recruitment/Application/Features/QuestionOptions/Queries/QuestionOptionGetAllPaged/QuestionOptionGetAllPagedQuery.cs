using MediatR;
using SylviaNG.Recruitment.Application.Features.QuestionOptions.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.QuestionOptions.Queries.QuestionOptionGetAllPaged
{
    public class QuestionOptionGetAllPagedQuery : IRequest<PagedResult<QuestionOptionResponse>>
    {
        public PagedRequest Request { get; set; }

        public QuestionOptionGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

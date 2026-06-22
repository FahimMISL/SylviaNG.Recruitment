using MediatR;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Queries.QuestionGroupGetAllPaged
{
    public class QuestionGroupGetAllPagedQuery : IRequest<PagedResult<QuestionGroupResponse>>
    {
        public PagedRequest Request { get; set; }

        public QuestionGroupGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewSessions.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.InterviewSessions.Queries.InterviewSessionGetAllPaged
{
    public class InterviewSessionGetAllPagedQuery : IRequest<PagedResult<InterviewSessionResponse>>
    {
        public PagedRequest Request { get; set; }

        public InterviewSessionGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

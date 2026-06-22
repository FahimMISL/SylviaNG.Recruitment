using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamCandidates.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ExamCandidates.Queries.ExamCandidateGetAllPaged
{
    public class ExamCandidateGetAllPagedQuery : IRequest<PagedResult<ExamCandidateResponse>>
    {
        public PagedRequest Request { get; set; }

        public ExamCandidateGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Features.Interviews.Models;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Queries.InterviewGetAllPaged
{
    public class InterviewGetAllPagedQuery : IRequest<PagedResult<InterviewResponse>>
    {
        public PagedRequest Request { get; set; }
        public long? JobPostingId { get; set; }
        public InterviewStatusEnum? Status { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public InterviewGetAllPagedQuery(PagedRequest request, long? jobPostingId, InterviewStatusEnum? status, DateTime? dateFrom, DateTime? dateTo)
        {
            Request = request;
            JobPostingId = jobPostingId;
            Status = status;
            DateFrom = dateFrom;
            DateTo = dateTo;
        }
    }
}

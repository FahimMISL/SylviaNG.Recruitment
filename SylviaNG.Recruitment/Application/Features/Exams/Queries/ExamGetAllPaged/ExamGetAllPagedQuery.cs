using MediatR;
using SylviaNG.Recruitment.Application.Features.Exams.Models;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.Exams.Queries.ExamGetAllPaged
{
    public class ExamGetAllPagedQuery : IRequest<PagedResult<ExamResponse>>
    {
        public PagedRequest Request { get; set; }
        public long? JobPostingId { get; set; }
        public ExamTypeEnum? ExamType { get; set; }
        public bool? IsActive { get; set; }

        public ExamGetAllPagedQuery(PagedRequest request, long? jobPostingId, ExamTypeEnum? examType, bool? isActive)
        {
            Request = request;
            JobPostingId = jobPostingId;
            ExamType = examType;
            IsActive = isActive;
        }
    }
}

using SylviaNG.Recruitment.Application.Features.Exams.Models;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IExamService
    {
        Task<long> CreateAsync(ExamCreateRequest request);
        Task<PagedResult<ExamResponse>> GetPagedAsync(PagedRequest request, long? jobPostingId, ExamTypeEnum? examType, bool? isActive);
        Task<ExamResponse> GetByIdAsync(long examId);
    }
}

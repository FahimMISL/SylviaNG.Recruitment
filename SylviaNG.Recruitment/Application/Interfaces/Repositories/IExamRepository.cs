using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IExamRepository : IRepository<Exam>
    {
        Task<PagedResult<Exam>> GetPagedAsync(PagedRequest request, long? jobPostingId, ExamTypeEnum? examType, bool? isActive);

        /// <summary>Full aggregate load: exam plus venue/question-group, for read-heavy use (get-by-id, seat-plan generation).</summary>
        Task<Exam?> GetByIdWithDetailsAsync(long examId);
    }
}

using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IQuestionGroupService
    {
        Task<long> CreateAsync(QuestionGroupCreateRequest request);
        Task UpdateAsync(long questionGroupId, QuestionGroupUpdateRequest request);
        Task DeleteAsync(long questionGroupId);
        Task<List<QuestionGroupResponse>> GetAllAsync();
        Task<QuestionGroupResponse> GetByIdAsync(long questionGroupId);
        Task<PagedResult<QuestionGroupResponse>> GetPaginatedAsync(PagedRequest request);
    }
}

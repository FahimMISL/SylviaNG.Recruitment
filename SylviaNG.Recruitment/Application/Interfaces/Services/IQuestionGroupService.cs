using SylviaNG.Recruitment.Application.Features.QuestionGroups.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IQuestionGroupService
    {
        Task<long> CreateAsync(QuestionGroupCreateRequest request);
        Task UpdateAsync(long questionGroupId, QuestionGroupUpdateRequest request);
        Task SetActiveStatusAsync(long questionGroupId, bool isActive);
        Task<QuestionGroupResponse> GetByIdAsync(long questionGroupId);
        Task<List<QuestionGroupResponse>> GetAllAsync();
        Task<List<QuestionGroupLookupResponse>> GetActiveLookupAsync();
    }
}

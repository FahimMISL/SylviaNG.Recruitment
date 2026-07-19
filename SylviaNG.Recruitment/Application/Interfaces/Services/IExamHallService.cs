using SylviaNG.Recruitment.Application.Features.ExamHalls.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IExamHallService
    {
        Task<long> CreateAsync(ExamHallCreateRequest request);
        Task UpdateAsync(long examHallId, ExamHallUpdateRequest request);
        Task SetActiveStatusAsync(long examHallId, bool isActive);
        Task<ExamHallResponse> GetByIdAsync(long examHallId);
        Task<List<ExamHallResponse>> GetAllAsync();
        Task<List<ExamHallLookupResponse>> GetActiveLookupAsync();
    }
}

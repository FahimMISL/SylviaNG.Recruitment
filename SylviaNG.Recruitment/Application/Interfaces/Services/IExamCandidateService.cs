using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ExamCandidates.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IExamCandidateService
    {
        Task<long> CreateAsync(ExamCandidateCreateRequest request);
        Task UpdateAsync(long examCandidateId, ExamCandidateUpdateRequest request);
        Task DeleteAsync(long examCandidateId);
        Task<List<ExamCandidateResponse>> GetAllAsync();
        Task<ExamCandidateResponse> GetByIdAsync(long examCandidateId);
        Task<PagedResult<ExamCandidateResponse>> GetPaginatedAsync(PagedRequest request);
    }
}

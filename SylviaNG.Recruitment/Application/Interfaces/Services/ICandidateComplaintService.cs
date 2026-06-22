using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.CandidateComplaints.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ICandidateComplaintService
    {
        Task<long> CreateAsync(CandidateComplaintCreateRequest request);
        Task UpdateAsync(long candidateComplaintId, CandidateComplaintUpdateRequest request);
        Task DeleteAsync(long candidateComplaintId);
        Task<List<CandidateComplaintResponse>> GetAllAsync();
        Task<CandidateComplaintResponse> GetByIdAsync(long candidateComplaintId);
        Task<PagedResult<CandidateComplaintResponse>> GetPaginatedAsync(PagedRequest request);
    }
}

using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ICandidateCertificationService
    {
        Task<List<CandidateCertificationResponse>> GetAllForCurrentCandidateAsync();
        Task<long> CreateAsync(CandidateCertificationCreateRequest request);
        Task UpdateAsync(long candidateCertificationId, CandidateCertificationUpdateRequest request);
        Task DeleteAsync(long candidateCertificationId);
    }
}

using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ICandidateEducationService
    {
        Task<List<CandidateEducationResponse>> GetAllForCurrentCandidateAsync();
        Task<long> CreateAsync(CandidateEducationCreateRequest request);
        Task UpdateAsync(long candidateEducationId, CandidateEducationUpdateRequest request);
        Task DeleteAsync(long candidateEducationId);
    }
}

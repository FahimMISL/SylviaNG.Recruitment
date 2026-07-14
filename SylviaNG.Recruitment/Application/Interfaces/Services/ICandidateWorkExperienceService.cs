using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ICandidateWorkExperienceService
    {
        Task<List<CandidateWorkExperienceResponse>> GetAllForCurrentCandidateAsync();
        Task<long> CreateAsync(CandidateWorkExperienceCreateRequest request);
        Task UpdateAsync(long candidateWorkExperienceId, CandidateWorkExperienceUpdateRequest request);
        Task DeleteAsync(long candidateWorkExperienceId);
    }
}

using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ICandidateSkillService
    {
        Task<List<CandidateSkillResponse>> GetAllForCurrentCandidateAsync();
        Task<long> CreateAsync(CandidateSkillCreateRequest request);
        Task DeleteAsync(long candidateSkillId);
    }
}

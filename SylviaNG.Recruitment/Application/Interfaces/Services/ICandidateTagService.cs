using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>HR-only candidate tagging (US-041) - distinct from ICandidateSkillService, which
    /// is candidate self-service via ICurrentCandidateService. HR tags someone else's profile,
    /// so every method takes an explicit candidateProfileId.</summary>
    public interface ICandidateTagService
    {
        Task<List<CandidateTagResponse>> GetAllAsync(long candidateProfileId);
        Task<long> CreateAsync(long candidateProfileId, CandidateTagCreateRequest request);
        Task DeleteAsync(long candidateProfileId, long candidateTagId);
        Task<List<string>> GetSuggestionsAsync(string? search);
    }
}

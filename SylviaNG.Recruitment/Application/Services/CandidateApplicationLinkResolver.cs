using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Services
{
    public class CandidateApplicationLinkResolver : ICandidateApplicationLinkResolver
    {
        private readonly ICurrentCandidateService _currentCandidateService;
        private readonly ICandidateProfileRepository _candidateProfileRepository;

        public CandidateApplicationLinkResolver(
            ICurrentCandidateService currentCandidateService,
            ICandidateProfileRepository candidateProfileRepository)
        {
            _currentCandidateService = currentCandidateService;
            _candidateProfileRepository = candidateProfileRepository;
        }

        // Resolves the applicant's CandidateProfile at submission time instead of leaving every
        // downstream reader to match on CandidateEmail. Prefers the actually-authenticated
        // submitter's own profile (covers internal-board apply, and any logged-in candidate using
        // the guest endpoint); falls back to an existing profile matching the typed email; stays
        // null for a guest with no profile yet - it gets linked later at registration (see
        // CurrentCandidateService.GetOrCreateCurrentProfileAsync's claim step).
        public async Task<long?> ResolveCandidateProfileIdAsync(string? candidateEmail)
        {
            var currentProfileId = await _currentCandidateService.TryGetCurrentCandidateProfileIdAsync();
            if (currentProfileId != null)
                return currentProfileId;

            return string.IsNullOrEmpty(candidateEmail)
                ? null
                : await _candidateProfileRepository.GetIdByEmailAsync(candidateEmail);
        }
    }
}

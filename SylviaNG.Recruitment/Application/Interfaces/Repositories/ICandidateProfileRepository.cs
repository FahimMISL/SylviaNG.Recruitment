using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface ICandidateProfileRepository : IRepository<CandidateProfile>
    {
        Task<CandidateProfile?> GetByKeycloakSubjectIdAsync(string keycloakSubjectId);

        /// <summary>
        /// Id of an existing profile matching this email (case-insensitive), or null if none
        /// exists yet - used to opportunistically link a guest job application to an
        /// already-registered candidate at submission time. Never creates a profile.
        /// </summary>
        Task<long?> GetIdByEmailAsync(string email);

        /// <summary>
        /// Paged, searchable candidate list for the HR/Admin view (US-009), optionally narrowed to
        /// members of the given talent pools (US-039 AC4) and/or candidates having any of the
        /// given tags (US-041 AC3, ANY-match semantics, same as Skills filtering elsewhere).
        /// </summary>
        Task<PagedResult<CandidateProfile>> GetPagedAsync(PagedRequest request, List<long>? talentPoolIds = null, List<string>? tags = null);

        /// <summary>
        /// Bulk-resolve profiles by email, with education/work-experience/skills included, for
        /// shortlist filter evaluation (US-043). JobApplication has no FK to CandidateProfile,
        /// so this is matched by CandidateEmail - same join precedent as US-040.
        /// </summary>
        Task<List<CandidateProfile>> GetByEmailsAsync(IEnumerable<string> emails);

        /// <summary>
        /// Every active profile with all sections included (education/work-experience/skills/
        /// certifications/documents), for CV Bank's in-memory Boolean search evaluation (US-045)
        /// - same full-scan precedent as GetByEmailsAsync/ShortlistFilterEvaluationService.
        /// </summary>
        Task<List<CandidateProfile>> GetAllActiveWithDetailsAsync();
    }
}

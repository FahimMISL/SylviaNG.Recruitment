using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface ICandidateProfileRepository : IRepository<CandidateProfile>
    {
        Task<CandidateProfile?> GetByKeycloakSubjectIdAsync(string keycloakSubjectId);

        /// <summary>Paged, searchable candidate list for the HR/Admin view (US-009). Optionally
        /// narrowed to candidates having any of the given tags (US-041 AC3, ANY-match semantics,
        /// same as Skills filtering elsewhere).</summary>
        Task<PagedResult<CandidateProfile>> GetPagedAsync(PagedRequest request, List<string>? tags = null);

        /// <summary>
        /// Bulk-resolve profiles by email, with education/work-experience/skills included, for
        /// shortlist filter evaluation (US-043). JobApplication has no FK to CandidateProfile,
        /// so this is matched by CandidateEmail - same join precedent as US-040.
        /// </summary>
        Task<List<CandidateProfile>> GetByEmailsAsync(IEnumerable<string> emails);
    }
}

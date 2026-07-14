using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface ICandidateProfileRepository : IRepository<CandidateProfile>
    {
        Task<CandidateProfile?> GetByKeycloakSubjectIdAsync(string keycloakSubjectId);

        /// <summary>Paged, searchable candidate list for the HR/Admin view (US-009).</summary>
        Task<PagedResult<CandidateProfile>> GetPagedAsync(PagedRequest request);
    }
}

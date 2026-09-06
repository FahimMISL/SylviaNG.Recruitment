using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IJobPostingRepository : IRepository<JobPosting>
    {
        Task<bool> ExistsByTitleAsync(string title, long? excludeId = null);
        Task<PagedResult<JobPosting>> GetPaginatedAsync(PagedRequest request);

        /// <summary>Full (non-paginated) list, newest-created first.</summary>
        Task<List<JobPosting>> GetAllNewestFirstAsync();

        /// <summary>
        /// Paginated, audience-filtered browse used by the career portal (external/public) and
        /// internal job board. Only Open postings whose CircularType is in <paramref name="allowedCircularTypes"/>
        /// are returned, further narrowed by the optional filters when provided.
        /// </summary>
        /// <param name="ignoreCompanyScope">
        /// True to lift the ICompanyScoped global query filter (via IgnoreQueryFilters) so the
        /// caller sees postings across every company - used by the public career portal, where a
        /// candidate applying to any company must be able to browse all of them.
        /// </param>
        Task<PagedResult<JobPosting>> GetPaginatedByCircularTypesAsync(
            PagedRequest request,
            IReadOnlyCollection<CircularTypeEnum> allowedCircularTypes,
            string? location,
            long? departmentId,
            EmploymentTypeEnum? employmentType,
            int? maxExperienceYears,
            bool ignoreCompanyScope = false);

        /// <summary>
        /// Audience-filtered single-posting lookup for the career portal / internal job board detail views.
        /// Returns null (rather than exposing existence) when the posting is not Open or its CircularType
        /// is not in <paramref name="allowedCircularTypes"/>.
        /// </summary>
        /// <param name="ignoreCompanyScope">
        /// True to lift the ICompanyScoped global query filter so a public career-portal visitor
        /// can open a posting owned by any company (see <see cref="GetPaginatedByCircularTypesAsync"/>).
        /// </param>
        Task<JobPosting?> GetOpenByIdAndCircularTypesAsync(long jobPostingId, IReadOnlyCollection<CircularTypeEnum> allowedCircularTypes, bool ignoreCompanyScope = false);

        /// <summary>Count of postings with the given status, for dashboard summary stats.</summary>
        Task<int> CountByStatusAsync(JobStatusEnum status);

        /// <summary>EP-15/US-113: postings created by the given local UserAccountId, for the "My Postings" view.</summary>
        Task<List<JobPosting>> GetByCreatedByAsync(long userAccountId);

        /// <summary>Application count per posting, for the given ids only - one grouped query
        /// instead of loading every JobApplication row via .Include(j => j.Applications) just to
        /// read Count. A posting with zero applications has no key in the result.</summary>
        Task<Dictionary<long, int>> GetApplicationCountsByJobPostingIdsAsync(IReadOnlyCollection<long> jobPostingIds);
    }
}

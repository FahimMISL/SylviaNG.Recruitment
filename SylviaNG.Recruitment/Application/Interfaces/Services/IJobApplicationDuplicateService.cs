using SylviaNG.Recruitment.Application.Features.JobPostings.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IJobApplicationDuplicateService
    {
        /// <summary>
        /// Groups applications to the given vacancy that share an email, national ID, or phone
        /// number across different applications, so HR can spot cross-channel duplicates (US-038 AC1/AC2).
        /// Already-DuplicateDismissed applications are excluded from re-surfacing as open groups.
        /// </summary>
        Task<List<JobApplicationDuplicateGroupResponse>> GetDuplicatesAsync(long jobPostingId);

        /// <summary>
        /// HR keeps one application as primary and dismisses the rest of a detected duplicate
        /// group (US-038 AC3/AC4). Re-validates the group server-side; dismissed applications
        /// move to DuplicateDismissed with an audit trail entry, retained for history.
        /// </summary>
        Task ResolveDuplicatesAsync(JobApplicationDuplicateResolveRequest request);
    }
}

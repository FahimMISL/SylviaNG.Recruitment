using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>Status transitions, bulk status/notify/CV-download operations. GetStatusReasonsAsync
    /// is grouped here (not on IJobApplicationCoreService) since it's tagged the same US-036 story
    /// as UpdateStatusAsync and only ever feeds that action's reason dropdown.</summary>
    public interface IJobApplicationStatusService
    {
        /// <summary>Reject/withdraw reasons for the given status, for the dropdown (US-036 AC3).</summary>
        Task<List<ApplicationStatusReasonResponse>> GetStatusReasonsAsync(ApplicationStatusEnum status);

        /// <summary>Moves a single application to a new status, validating the transition and reason (US-036).</summary>
        Task UpdateStatusAsync(long jobApplicationId, JobApplicationStatusUpdateRequest request);

        /// <summary>Best-effort bulk status move across multiple applications (US-035 AC5).</summary>
        Task<JobApplicationBulkStatusUpdateResponse> BulkUpdateStatusAsync(JobApplicationBulkStatusUpdateRequest request);

        /// <summary>Best-effort re-dispatch of a chosen event's notification across multiple applications (US-076).</summary>
        Task<JobApplicationBulkNotifyResponse> BulkNotifyAsync(JobApplicationBulkNotifyRequest request);

        /// <summary>
        /// US-101: synchronous ZIP of the selected applications' system-rendered CVs, one PDF per
        /// application. Capped at BulkDownloadCvsSyncMaxCount - larger batches go through the
        /// EP-13 F1 async export queue instead (IExportRequestService.RequestBulkCvZipExportAsync).
        /// Applications with no linked CandidateProfile (guest applicants) are silently skipped.
        /// </summary>
        Task<JobApplicationCvBulkDownloadResponse> BulkDownloadCvsAsync(JobApplicationCvBulkDownloadRequest request);

        /// <summary>
        /// Applies a validated status transition to an already-loaded entity WITHOUT calling
        /// SaveChangesAsync - UpdateStatusAsync/BulkUpdateStatusAsync each wrap this with their own
        /// save; JobApplicationDuplicateService.ResolveDuplicatesAsync calls this directly in a loop
        /// and saves once at the end, matching what this method did as a private helper before the
        /// service was split. Not for use outside a caller that owns its own SaveChangesAsync.
        /// </summary>
        Task ApplyStatusChangeAsync(JobApplication entity, JobApplicationStatusUpdateRequest request);
    }
}

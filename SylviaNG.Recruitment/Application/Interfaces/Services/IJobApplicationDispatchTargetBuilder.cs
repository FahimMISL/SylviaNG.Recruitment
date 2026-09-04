using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>
    /// Builds the NotificationDispatchTargets for a JobApplication-triggered event. Shared by
    /// JobApplicationSubmissionService, JobApplicationStatusService and JobApplicationSelfService -
    /// extracted out of the former monolithic JobApplicationService so the HR-fan-out targeting
    /// logic (in particular the explicit CompanyId, needed because several of these call sites run
    /// with no ambient tenant scope) lives in exactly one place.
    /// </summary>
    public interface IJobApplicationDispatchTargetBuilder
    {
        Task<NotificationDispatchTargets> BuildTargetsAsync(JobApplication entity);
    }
}

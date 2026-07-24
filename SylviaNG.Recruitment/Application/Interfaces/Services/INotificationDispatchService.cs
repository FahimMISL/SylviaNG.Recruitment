using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>Which concrete address each recipient type resolves to for one DispatchAsync call.
    /// A null address means that recipient type's leg is Skipped (no email on file / HR mailbox not
    /// configured), not an error.</summary>
    public record NotificationDispatchTargets(string? CandidateEmail, string? AdminHrEmail, long? JobApplicationId = null);

    /// <summary>
    /// EP-09 Feature 2: the real consumer of Feature 1's EventTemplateMapping/NotificationTemplate
    /// tables - resolves a mapping per recipient type, renders it via IPlaceholderSubstitutionService,
    /// and sends via ISmtpEmailService. Deliberately never throws (mirrors ExamNotificationService):
    /// a missing mapping, a render issue, or an SMTP failure all end up as a NotificationLog row
    /// instead of surfacing to the caller.
    /// </summary>
    public interface INotificationDispatchService
    {
        /// <summary>
        /// <paramref name="persistImmediately"/> controls whether this call issues its own
        /// SaveChangesAsync (true - standalone bulk-notify use) or leaves persistence to the
        /// caller's own SaveChangesAsync later in the same unit of work (false - inline calls from
        /// JobApplicationService, so the status change and the notification log commit atomically).
        /// </summary>
        Task DispatchAsync(
            RecruitmentEventEnum recruitmentEvent,
            IDictionary<string, string> placeholderValues,
            NotificationDispatchTargets targets,
            bool persistImmediately = true,
            CancellationToken cancellationToken = default);
    }
}

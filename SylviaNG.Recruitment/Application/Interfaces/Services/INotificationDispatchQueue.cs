using SylviaNG.Recruitment.Application.Common.Email;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>One queued DispatchAsync call, carrying everything the worker needs to replay it
    /// in its own scope. PlaceholderValues is copied by the queue implementation so a caller that
    /// keeps mutating its dictionary after enqueuing can't change what gets rendered.</summary>
    public record NotificationDispatchRequest(
        RecruitmentEventEnum RecruitmentEvent,
        IDictionary<string, string> PlaceholderValues,
        NotificationDispatchTargets Targets,
        IReadOnlyList<EmailAttachment>? Attachments = null);

    /// <summary>
    /// Hands a notification off to NotificationDispatchWorker instead of sending it inline.
    ///
    /// Motivation (EP-17 payment path): DispatchAsync opens a full SMTP session - TCP + STARTTLS +
    /// AUTH + send - per recipient, sequentially, wrapped in EmailRetrySender's 3 bounded attempts.
    /// Awaited inline that put 1..N Gmail round trips between the applicant clicking Submit and the
    /// gateway page opening, and again between SSLCommerz's browser-return callback and the 302
    /// back to the result page. Neither response has any reason to wait on mail delivery.
    ///
    /// In-memory only, deliberately: this codebase has no durable job store (no Hangfire/Quartz/
    /// Polly - see EmailRetrySender's remarks), so a process restart with items still queued drops
    /// them, exactly as an in-flight inline send would have been dropped. Callers that must know
    /// the send outcome (ExamNotificationService, InterviewNotificationService persist per-entity
    /// send status) keep calling INotificationDispatchService directly.
    /// </summary>
    public interface INotificationDispatchQueue
    {
        /// <summary>Never throws and never blocks. Returns false only if the queue is saturated,
        /// in which case the notification is dropped and logged rather than stalling the request.</summary>
        bool TryEnqueue(NotificationDispatchRequest request);
    }
}

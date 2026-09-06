using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface INotificationLogRepository : IRepository<NotificationLog>
    {
        /// <summary>EP-09 Feature 3 (US-078): unfiltered fields are left null. Includes every
        /// RecipientType - the HR log view covers both candidate-facing and internal sends.</summary>
        IQueryable<NotificationLog> GetFilteredQueryable(
            DateTime? fromDate,
            DateTime? toDate,
            NotificationChannelEnum? channel,
            RecruitmentEventEnum? recruitmentEvent,
            NotificationStatusEnum? deliveryStatus);

        /// <summary>US-079 AC2: most recent unread rows for the HR/Admin bell, newest first.</summary>
        Task<List<NotificationLog>> GetUnreadForAdminHrAsync(int take);

        /// <summary>US-079 AC1/AC5: badge count, polled.</summary>
        Task<int> GetUnreadCountForAdminHrAsync();

        /// <summary>US-079 AC4 "Mark all as read". Returns the number of rows updated.</summary>
        Task<int> MarkAllAsReadForAdminHrAsync();

        /// <summary>Candidate-facing bell: most recent unread rows addressed to this candidate's
        /// own applications, newest first. Scoped by JobApplication.CandidateProfileId, not
        /// RecipientAddress, so it survives an email change (see the Account Settings email-change
        /// feature) instead of silently going stale.</summary>
        Task<List<NotificationLog>> GetUnreadForCandidateAsync(long candidateProfileId, int take);

        /// <summary>Candidate-facing badge count, polled.</summary>
        Task<int> GetUnreadCountForCandidateAsync(long candidateProfileId);

        /// <summary>Candidate-facing "mark all as read". Returns the number of rows updated.</summary>
        Task<int> MarkAllAsReadForCandidateAsync(long candidateProfileId);

        /// <summary>IDOR guard for the candidate-facing mark-as-read endpoint - true only if this
        /// row belongs to one of the given candidate's own applications.</summary>
        Task<bool> IsOwnedByCandidateAsync(long notificationLogId, long candidateProfileId);
    }
}

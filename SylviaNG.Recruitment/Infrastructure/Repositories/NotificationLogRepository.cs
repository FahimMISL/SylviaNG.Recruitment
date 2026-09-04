using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class NotificationLogRepository : Repository<NotificationLog>, INotificationLogRepository
    {
        public NotificationLogRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public IQueryable<NotificationLog> GetFilteredQueryable(
            DateTime? fromDate,
            DateTime? toDate,
            NotificationChannelEnum? channel,
            RecruitmentEventEnum? recruitmentEvent,
            NotificationStatusEnum? deliveryStatus)
        {
            var query = _dbSet
                .AsNoTracking()
                .Include(l => l.JobApplication)
                    .ThenInclude(ja => ja!.CandidateProfile)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(l => l.CreatedAt >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(l => l.CreatedAt <= toDate.Value);
            if (channel.HasValue)
                query = query.Where(l => l.Channel == channel.Value);
            if (recruitmentEvent.HasValue)
                query = query.Where(l => l.RecruitmentEvent == recruitmentEvent.Value);
            if (deliveryStatus.HasValue)
                query = query.Where(l => l.DeliveryStatus == deliveryStatus.Value);

            return query.OrderByDescending(l => l.CreatedAt);
        }

        public async Task<List<NotificationLog>> GetUnreadForAdminHrAsync(int take)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(l => l.RecipientType == NotificationRecipientTypeEnum.AdminHr && !l.IsRead)
                .OrderByDescending(l => l.CreatedAt)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountForAdminHrAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .CountAsync(l => l.RecipientType == NotificationRecipientTypeEnum.AdminHr && !l.IsRead);
        }

        public async Task<int> MarkAllAsReadForAdminHrAsync()
        {
            return await _dbSet
                .Where(l => l.RecipientType == NotificationRecipientTypeEnum.AdminHr && !l.IsRead)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(l => l.IsRead, true)
                    .SetProperty(l => l.ReadAt, DateTime.UtcNow));
        }

        public async Task<List<NotificationLog>> GetUnreadForCandidateAsync(long candidateProfileId, int take)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(l => l.JobApplication)
                    .ThenInclude(ja => ja!.CandidateProfile)
                .Where(l => l.RecipientType == NotificationRecipientTypeEnum.Candidate
                    && !l.IsRead
                    && l.JobApplication != null
                    && l.JobApplication.CandidateProfileId == candidateProfileId)
                .OrderByDescending(l => l.CreatedAt)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountForCandidateAsync(long candidateProfileId)
        {
            return await _dbSet
                .AsNoTracking()
                .CountAsync(l => l.RecipientType == NotificationRecipientTypeEnum.Candidate
                    && !l.IsRead
                    && l.JobApplication != null
                    && l.JobApplication.CandidateProfileId == candidateProfileId);
        }

        public async Task<int> MarkAllAsReadForCandidateAsync(long candidateProfileId)
        {
            return await _dbSet
                .Where(l => l.RecipientType == NotificationRecipientTypeEnum.Candidate
                    && !l.IsRead
                    && l.JobApplication != null
                    && l.JobApplication.CandidateProfileId == candidateProfileId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(l => l.IsRead, true)
                    .SetProperty(l => l.ReadAt, DateTime.UtcNow));
        }

        public async Task<bool> IsOwnedByCandidateAsync(long notificationLogId, long candidateProfileId)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(l => l.NotificationLogId == notificationLogId
                    && l.RecipientType == NotificationRecipientTypeEnum.Candidate
                    && l.JobApplication != null
                    && l.JobApplication.CandidateProfileId == candidateProfileId);
        }
    }
}

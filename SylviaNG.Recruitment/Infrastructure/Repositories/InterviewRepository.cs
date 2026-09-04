using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class InterviewRepository : Repository<Interview>, IInterviewRepository
    {
        public InterviewRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<PagedResult<Interview>> GetPagedAsync(
            PagedRequest request,
            long? jobPostingId,
            InterviewStatusEnum? status,
            DateTime? dateFrom,
            DateTime? dateTo)
        {
            var query = _dbSet
                .Include(i => i.JobApplication)
                .Include(i => i.InterviewVenue)
                .Include(i => i.InterviewRoom)
                .Include(i => i.PanelMembers)
                    .ThenInclude(p => p.Employee)
                .AsQueryable();

            if (jobPostingId.HasValue)
                query = query.Where(i => i.JobApplication.JobPostingId == jobPostingId.Value);

            if (status.HasValue)
                query = query.Where(i => i.Status == status.Value);

            if (dateFrom.HasValue)
                query = query.Where(i => i.ScheduledStartAt >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(i => i.ScheduledStartAt <= dateTo.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                // Interview has no searchable text field of its own - the generic
                // ToPaginatedResultAsync SearchProperties mechanism only reflects over direct
                // properties of T, so a cross-entity search on the linked candidate needs an
                // explicit Where instead.
                var normalizedSearchTerm = request.SearchTerm.Trim().ToLowerInvariant();

                query = query.Where(i =>
                    i.JobApplication.CandidateName.ToLower().Contains(normalizedSearchTerm) ||
                    (i.JobApplication.CandidateEmail != null && i.JobApplication.CandidateEmail.ToLower().Contains(normalizedSearchTerm)));
            }

            var orderedQuery = query.OrderByDescending(i => i.ScheduledStartAt);

            return await orderedQuery.ToPaginatedResultAsync(request);
        }

        public async Task<Interview?> GetByIdWithDetailsAsync(long interviewId)
        {
            return await _dbSet
                .Include(i => i.JobApplication)
                .Include(i => i.InterviewVenue)
                .Include(i => i.InterviewRoom)
                .Include(i => i.InterviewRoundConfig)
                .Include(i => i.PanelMembers)
                    .ThenInclude(p => p.Employee)
                .FirstOrDefaultAsync(i => i.InterviewId == interviewId);
        }

        public async Task<List<Interview>> GetByIdsWithDetailsAsync(List<long> interviewIds)
        {
            return await _dbSet
                .Include(i => i.JobApplication)
                .Include(i => i.InterviewVenue)
                .Include(i => i.InterviewRoom)
                .Include(i => i.PanelMembers)
                    .ThenInclude(p => p.Employee)
                .Where(i => interviewIds.Contains(i.InterviewId))
                .ToListAsync();
        }

        public async Task<List<Interview>> GetByJobApplicationIdAsync(long jobApplicationId)
        {
            return await _dbSet
                .Include(i => i.InterviewVenue)
                .Include(i => i.InterviewRoom)
                .Include(i => i.InterviewRoundConfig)
                .Include(i => i.PanelMembers)
                    .ThenInclude(p => p.Employee)
                .Where(i => i.JobApplicationId == jobApplicationId)
                .OrderByDescending(i => i.ScheduledStartAt)
                .ToListAsync();
        }

        public async Task<int> GetOverlappingCountByRoomIdAsync(long interviewRoomId, DateTime start, DateTime end, long? excludeInterviewId = null)
        {
            return await _dbSet.CountAsync(i =>
                i.InterviewRoomId == interviewRoomId &&
                i.Status != InterviewStatusEnum.Cancelled &&
                i.ScheduledStartAt < end &&
                i.ScheduledEndAt > start &&
                (!excludeInterviewId.HasValue || i.InterviewId != excludeInterviewId.Value));
        }

        public async Task<List<long>> GetConflictingPanelistEmployeeIdsAsync(List<long> employeeIds, DateTime start, DateTime end, long? excludeInterviewId = null)
        {
            return await _dbSet
                .Where(i =>
                    i.Status != InterviewStatusEnum.Cancelled &&
                    i.ScheduledStartAt < end &&
                    i.ScheduledEndAt > start &&
                    (!excludeInterviewId.HasValue || i.InterviewId != excludeInterviewId.Value))
                .SelectMany(i => i.PanelMembers)
                .Where(p => employeeIds.Contains(p.EmployeeId))
                .Select(p => p.EmployeeId)
                .Distinct()
                .ToListAsync();
        }

        public async Task<bool> ExistsPassedForRoundConfigAsync(long jobApplicationId, long roundConfigId)
        {
            return await _dbSet.AnyAsync(i =>
                i.JobApplicationId == jobApplicationId &&
                i.InterviewRoundConfigId == roundConfigId &&
                i.Result == InterviewResultEnum.Passed);
        }

        public async Task<bool> ExistsForRoundConfigAsync(long roundConfigId)
        {
            return await _dbSet.AnyAsync(i => i.InterviewRoundConfigId == roundConfigId);
        }

        public async Task<int> CountUpcomingAsync(DateTime now)
        {
            return await _dbSet.CountAsync(i =>
                (i.Status == InterviewStatusEnum.Scheduled || i.Status == InterviewStatusEnum.Rescheduled) &&
                i.ScheduledStartAt >= now);
        }
    }
}

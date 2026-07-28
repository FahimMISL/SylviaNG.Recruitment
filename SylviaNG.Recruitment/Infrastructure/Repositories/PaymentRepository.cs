using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Features.PaymentReports.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        private IQueryable<Payment> BuildTransactionFilterQuery(PaymentTransactionFilterRequest filter)
        {
            return _dbSet
                .Include(p => p.JobApplication).ThenInclude(a => a.JobPosting)
                .Where(p => filter.JobPostingId == null || p.JobApplication.JobPostingId == filter.JobPostingId)
                .Where(p => filter.PaymentStatus == null || p.PaymentStatus == filter.PaymentStatus)
                .Where(p => string.IsNullOrEmpty(filter.CandidateName) || p.JobApplication.CandidateName.Contains(filter.CandidateName))
                .Where(p => string.IsNullOrEmpty(filter.CandidateEmail) || (p.JobApplication.CandidateEmail != null && p.JobApplication.CandidateEmail.Contains(filter.CandidateEmail)))
                .Where(p => filter.DateFrom == null || (p.PaidAt ?? p.JobApplication.AppliedDate) >= filter.DateFrom)
                .Where(p => filter.DateTo == null || (p.PaidAt ?? p.JobApplication.AppliedDate) <= filter.DateTo);
        }

        public async Task<PagedResult<Payment>> GetPagedTransactionsAsync(PaymentTransactionFilterRequest filter, PagedRequest paging)
        {
            var query = BuildTransactionFilterQuery(filter).OrderByDescending(p => p.PaymentId);
            return await query.ToPaginatedResultAsync(paging);
        }

        public async Task<decimal> GetFilteredTotalAmountAsync(PaymentTransactionFilterRequest filter)
        {
            return await BuildTransactionFilterQuery(filter).SumAsync(p => (decimal?)p.Amount) ?? 0m;
        }

        public async Task<List<Payment>> GetLatestPaymentsInScopeAsync(DateTime from, DateTime to, long? jobPostingId, long? departmentId, long? siteId)
        {
            var payments = await _dbSet
                .Include(p => p.JobApplication).ThenInclude(a => a.JobPosting)
                .Where(p => p.JobApplication.AppliedDate != null && p.JobApplication.AppliedDate >= from && p.JobApplication.AppliedDate <= to)
                .Where(p => jobPostingId == null || p.JobApplication.JobPostingId == jobPostingId)
                .Where(p => departmentId == null || p.JobApplication.JobPosting.DepartmentId == departmentId)
                .Where(p => siteId == null || p.JobApplication.JobPosting.SiteId == siteId)
                .ToListAsync();

            // Grouped client-side (period-bounded dataset) to avoid relying on provider support
            // for GroupBy+First() translation - same "latest attempt wins" rule as
            // GetLatestByJobApplicationIdAsync, applied per application within scope.
            return payments
                .GroupBy(p => p.JobApplicationId)
                .Select(g => g.OrderByDescending(p => p.PaymentId).First())
                .ToList();
        }

        public async Task<Payment?> GetLatestByJobApplicationIdAsync(long jobApplicationId)
        {
            // PaymentId (identity, monotonic per insert), not CreatedAt - the audit-stamping
            // interceptor isn't wired up yet so CreatedAt is null on every row app-wide, and
            // ORDER BY a null column is non-deterministic in Postgres.
            return await _dbSet
                .Where(p => p.JobApplicationId == jobApplicationId)
                .OrderByDescending(p => p.PaymentId)
                .FirstOrDefaultAsync();
        }

        public async Task<Payment?> GetByTransactionIdAsync(string transactionId)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.TransactionId == transactionId);
        }

        public async Task<bool> HasSuccessfulPaymentAsync(long jobApplicationId)
        {
            return await _dbSet.AnyAsync(p =>
                p.JobApplicationId == jobApplicationId && p.PaymentStatus == PaymentStatusEnum.Success);
        }
    }
}

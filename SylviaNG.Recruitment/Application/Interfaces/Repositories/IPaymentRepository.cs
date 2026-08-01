using SylviaNG.Recruitment.Application.Features.PaymentReports.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        /// <summary>Most recent payment attempt for a JobApplication (by CreatedAt), or null if none exist.</summary>
        Task<Payment?> GetLatestByJobApplicationIdAsync(long jobApplicationId);

        Task<Payment?> GetByTransactionIdAsync(string transactionId);

        /// <summary>Whether a Success payment already exists for this application (used to reject duplicate initiate/IPN processing).</summary>
        Task<bool> HasSuccessfulPaymentAsync(long jobApplicationId);

        /// <summary>F1 transaction history list, with JobApplication/JobPosting included for candidate/vacancy columns.</summary>
        Task<PagedResult<Payment>> GetPagedTransactionsAsync(PaymentTransactionFilterRequest filter, PagedRequest paging);

        /// <summary>Sum of Amount across every transaction matching the filter, not just the current page.</summary>
        Task<decimal> GetFilteredTotalAmountAsync(PaymentTransactionFilterRequest filter);

        /// <summary>
        /// F1 reconciliation report: the latest attempt per JobApplication whose parent application
        /// was submitted in [from, to] and whose vacancy matches the given scope - avoids
        /// double-counting a failed-then-retried-successful application.
        /// </summary>
        Task<List<Payment>> GetLatestPaymentsInScopeAsync(DateTime from, DateTime to, long? jobPostingId, long? departmentId);
    }
}

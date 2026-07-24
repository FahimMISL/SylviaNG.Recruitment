using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        /// <summary>Most recent payment attempt for a JobApplication (by CreatedAt), or null if none exist.</summary>
        Task<Payment?> GetLatestByJobApplicationIdAsync(long jobApplicationId);

        Task<Payment?> GetByTransactionIdAsync(string transactionId);

        /// <summary>Whether a Success payment already exists for this application (used to reject duplicate initiate/IPN processing).</summary>
        Task<bool> HasSuccessfulPaymentAsync(long jobApplicationId);
    }
}

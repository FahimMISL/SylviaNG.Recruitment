using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDBContext dbContext) : base(dbContext) { }

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

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
            return await _dbSet
                .Where(p => p.JobApplicationId == jobApplicationId)
                .OrderByDescending(p => p.CreatedAt)
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

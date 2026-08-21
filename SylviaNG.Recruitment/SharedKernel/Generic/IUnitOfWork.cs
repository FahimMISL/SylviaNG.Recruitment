using SylviaNG.Recruitment.Infrastructure.Data;
using System.Data;

namespace SylviaNG.Recruitment.SharedKernel.Generic
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();

        /// <summary>isolationLevel is Unspecified (provider default) unless a caller needs
        /// stronger guarantees - e.g. Serializable to have Postgres detect a read-write conflict
        /// between two concurrent requests racing to update the same row (see
        /// PaymentService.HandleIpnAsync).</summary>
        Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        ApplicationDBContext Context { get; }
    }
}

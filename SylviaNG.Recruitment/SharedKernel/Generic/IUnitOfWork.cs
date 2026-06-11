using SylviaNG.Recruitment.Infrastructure.Data;

namespace SylviaNG.Recruitment.SharedKernel.Generic
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        ApplicationDBContext Context { get; }
    }
}

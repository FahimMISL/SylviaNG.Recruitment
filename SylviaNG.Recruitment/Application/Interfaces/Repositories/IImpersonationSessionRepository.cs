using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IImpersonationSessionRepository : IRepository<ImpersonationSession>
    {
        /// <summary>The actor's currently active (not ended, not expired) session, if any - at
        /// most one at a time per actor, enforced in ImpersonationService.</summary>
        Task<ImpersonationSession?> GetActiveByActorUserAccountIdAsync(long actorUserAccountId, DateTime nowUtc);
    }
}

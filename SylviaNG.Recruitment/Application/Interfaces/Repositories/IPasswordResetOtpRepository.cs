using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IPasswordResetOtpRepository : IRepository<PasswordResetOtp>
    {
        Task<PasswordResetOtp?> GetByChallengeIdAsync(Guid challengeId);
    }
}

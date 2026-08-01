using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface ICandidateLoginOtpRepository : IRepository<CandidateLoginOtp>
    {
        Task<CandidateLoginOtp?> GetByChallengeIdAsync(Guid challengeId);

        /// <summary>True once this username has ever completed one OTP challenge (ConsumedAtUtc
        /// set) - the OTP gate only applies to a candidate's first successful login, not every one.</summary>
        Task<bool> HasEverVerifiedAsync(string username);
    }
}

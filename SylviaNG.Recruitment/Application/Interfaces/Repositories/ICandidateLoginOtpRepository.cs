using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface ICandidateLoginOtpRepository : IRepository<CandidateLoginOtp>
    {
        Task<CandidateLoginOtp?> GetByChallengeIdAsync(Guid challengeId);
    }
}

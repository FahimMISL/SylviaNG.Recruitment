using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IUserInviteOtpRepository : IRepository<UserInviteOtp>
    {
        Task<UserInviteOtp?> GetByChallengeIdAsync(Guid challengeId);
    }
}

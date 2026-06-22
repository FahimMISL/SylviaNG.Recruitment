using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories;

public interface IUserProfilePhotoRepository : IRepository<UserProfilePhoto>
{
    Task<UserProfilePhoto?> GetByKeycloakUserIdAsync(string keycloakUserId);
}

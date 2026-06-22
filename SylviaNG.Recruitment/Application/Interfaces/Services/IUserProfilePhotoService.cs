using SylviaNG.Recruitment.Application.Features.UserProfilePhotos.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services;

public interface IUserProfilePhotoService
{
    Task<UserProfilePhotoResponse?> GetByKeycloakUserIdAsync(string keycloakUserId);
    Task<long> UploadAsync(string keycloakUserId, string fileName, string contentType, byte[] photoData);
    Task DeleteAsync(string keycloakUserId);
}

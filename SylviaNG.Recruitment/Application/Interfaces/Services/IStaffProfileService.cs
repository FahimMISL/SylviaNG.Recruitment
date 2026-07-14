using Microsoft.AspNetCore.Http;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>
    /// Photo-only account settings for Admin/HR users (see StaffProfile). Email/password are
    /// handled directly against Keycloak via IAccountSettingsService/IKeycloakClient.
    /// </summary>
    public interface IStaffProfileService
    {
        Task<string?> GetMyPhotoPathAsync();
        Task<string> UploadPhotoAsync(IFormFile file);
        Task DeletePhotoAsync();
    }
}

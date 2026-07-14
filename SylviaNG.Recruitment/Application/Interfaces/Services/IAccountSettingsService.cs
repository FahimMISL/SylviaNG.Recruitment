using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Features.AccountSettings.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>
    /// Common account settings (email, password, profile photo) shared by all 3 roles
    /// (Admin/HR/Candidate). Photo storage is delegated per-role: Candidate uses its existing
    /// CandidateProfile.ProfilePhotoPath, Admin/HR use the new StaffProfile.
    /// </summary>
    public interface IAccountSettingsService
    {
        Task<AccountSettingsResponse> GetMyAccountAsync();
        Task UpdateEmailAsync(AccountEmailUpdateRequest request);
        Task ChangePasswordAsync(AccountPasswordChangeRequest request);
        Task<string> UploadPhotoAsync(IFormFile file);
        Task DeletePhotoAsync();
    }
}

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

        /// <summary>Starts an email change: sends a one-time code to the NEW address. Keycloak is
        /// not touched until ConfirmEmailChangeAsync succeeds.</summary>
        Task<AccountEmailChangeChallengeResponse> RequestEmailChangeAsync(AccountEmailChangeRequest request);

        /// <summary>Verifies the code and, on success, applies the email change in Keycloak
        /// (marked verified) and - for Candidates - syncs CandidateProfile.Email. Returns the
        /// confirmed email.</summary>
        Task<string> ConfirmEmailChangeAsync(AccountEmailChangeConfirmRequest request);

        Task ChangePasswordAsync(AccountPasswordChangeRequest request);
        Task<string> UploadPhotoAsync(IFormFile file);
        Task DeletePhotoAsync();
    }
}

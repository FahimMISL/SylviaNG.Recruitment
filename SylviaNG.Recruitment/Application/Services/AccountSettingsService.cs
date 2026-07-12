using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Features.AccountSettings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Enums;
using System.Security.Claims;

namespace SylviaNG.Recruitment.Application.Services
{
    public class AccountSettingsService : IAccountSettingsService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IKeycloakClient _keycloakClient;
        private readonly ICandidateProfileService _candidateProfileService;
        private readonly IStaffProfileService _staffProfileService;

        public AccountSettingsService(
            IHttpContextAccessor httpContextAccessor,
            IKeycloakClient keycloakClient,
            ICandidateProfileService candidateProfileService,
            IStaffProfileService staffProfileService)
        {
            _httpContextAccessor = httpContextAccessor;
            _keycloakClient = keycloakClient;
            _candidateProfileService = candidateProfileService;
            _staffProfileService = staffProfileService;
        }

        public async Task<AccountSettingsResponse> GetMyAccountAsync()
        {
            var user = GetCurrentUser();
            var role = GetRole(user);
            var email = user.FindFirst(ClaimTypes.Email)?.Value ?? user.FindFirst("email")?.Value ?? string.Empty;

            var photoPath = role == UserRoleEnum.Candidate
                ? (await _candidateProfileService.GetMyProfileAsync()).ProfilePhotoPath
                : await _staffProfileService.GetMyPhotoPathAsync();

            return new AccountSettingsResponse
            {
                Email = email,
                ProfilePhotoPath = photoPath,
                Role = role.ToString()
            };
        }

        public async Task UpdateEmailAsync(AccountEmailUpdateRequest request)
        {
            var username = GetUsername(GetCurrentUser());
            var userId = await _keycloakClient.GetUserIdByUsernameAsync(username);
            await _keycloakClient.UpdateEmailAsync(userId, request.Email);
        }

        public async Task ChangePasswordAsync(AccountPasswordChangeRequest request)
        {
            var username = GetUsername(GetCurrentUser());

            // Verifies the current password by attempting a real login grant - throws
            // InvalidCredentialsException on mismatch (see IKeycloakClient.TokenAsync).
            await _keycloakClient.TokenAsync(username, request.CurrentPassword);

            var userId = await _keycloakClient.GetUserIdByUsernameAsync(username);
            await _keycloakClient.ResetPasswordAsync(userId, request.NewPassword);
        }

        public async Task<string> UploadPhotoAsync(IFormFile file)
        {
            var role = GetRole(GetCurrentUser());
            return role == UserRoleEnum.Candidate
                ? await _candidateProfileService.UploadPhotoAsync(file)
                : await _staffProfileService.UploadPhotoAsync(file);
        }

        public async Task DeletePhotoAsync()
        {
            var role = GetRole(GetCurrentUser());
            if (role == UserRoleEnum.Candidate)
                await _candidateProfileService.DeletePhotoAsync();
            else
                await _staffProfileService.DeletePhotoAsync();
        }

        private ClaimsPrincipal GetCurrentUser()
        {
            return _httpContextAccessor.HttpContext?.User
                ?? throw new UnauthorizedAccessException("No authenticated user in the current request.");
        }

        // Keycloak's "preferred_username" claim is the actual login username - ClaimTypes.Name
        // maps to the "name" (display name) claim for Keycloak tokens (see CurrentCandidateService),
        // so it is NOT usable here. The Local hardcoded-auth scheme sets ClaimTypes.Name to the
        // username itself, so that remains the fallback for that scheme.
        private static string GetUsername(ClaimsPrincipal user)
        {
            return user.FindFirst("preferred_username")?.Value
                ?? user.FindFirst(ClaimTypes.Name)?.Value
                ?? throw new UnauthorizedAccessException("Authenticated token does not carry a username claim.");
        }

        private static UserRoleEnum GetRole(ClaimsPrincipal user)
        {
            var roleValue = user.FindFirst(ClaimTypes.Role)?.Value;
            return Enum.TryParse<UserRoleEnum>(roleValue, true, out var role) ? role : UserRoleEnum.Candidate;
        }
    }
}

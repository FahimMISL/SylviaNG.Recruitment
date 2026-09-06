using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Extensions;
using SylviaNG.Recruitment.Application.Features.AccountSettings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SylviaNG.Recruitment.Application.Services
{
    public class AccountSettingsService : IAccountSettingsService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IKeycloakClient _keycloakClient;
        private readonly ICandidateProfileService _candidateProfileService;
        private readonly IStaffProfileService _staffProfileService;
        private readonly IEmailChangeVerificationRepository _emailChangeVerificationRepository;
        private readonly INotificationDispatchService _notificationDispatchService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly OtpSettings _otpSettings;
        private readonly ILogger<AccountSettingsService> _logger;

        public AccountSettingsService(
            IHttpContextAccessor httpContextAccessor,
            IKeycloakClient keycloakClient,
            ICandidateProfileService candidateProfileService,
            IStaffProfileService staffProfileService,
            IEmailChangeVerificationRepository emailChangeVerificationRepository,
            INotificationDispatchService notificationDispatchService,
            IUnitOfWork unitOfWork,
            IOptions<OtpSettings> otpSettings,
            ILogger<AccountSettingsService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _keycloakClient = keycloakClient;
            _candidateProfileService = candidateProfileService;
            _staffProfileService = staffProfileService;
            _emailChangeVerificationRepository = emailChangeVerificationRepository;
            _notificationDispatchService = notificationDispatchService;
            _unitOfWork = unitOfWork;
            _otpSettings = otpSettings.Value;
            _logger = logger;
        }

        public async Task<AccountSettingsResponse> GetMyAccountAsync()
        {
            var user = GetCurrentUser();
            var role = GetRole(user);
            var email = await GetLiveEmailAsync(user);

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

        // The JWT's email claim is baked in at token-issue time, so it goes stale the moment
        // UpdateEmailAsync changes the address in Keycloak - a page refresh re-reads the same
        // token and shows the old value until the user re-logs in. Fetch live from Keycloak
        // instead; fall back to the claim for the Local hardcoded-auth scheme (no Keycloak
        // identity at all) or if Keycloak is unreachable, since a display refresh shouldn't fail.
        private async Task<string> GetLiveEmailAsync(ClaimsPrincipal user)
        {
            var claimEmail = user.FindFirst(ClaimTypes.Email)?.Value ?? user.FindFirst("email")?.Value ?? string.Empty;

            try
            {
                var username = GetUsername(user);
                var userId = await _keycloakClient.GetUserIdByUsernameAsync(username);
                var liveEmail = await _keycloakClient.GetEmailByUserIdAsync(userId);
                return liveEmail ?? claimEmail;
            }
            catch (Exception ex) when (ex is KeycloakUnavailableException or NotFoundException)
            {
                return claimEmail;
            }
        }

        // Email change is a two-step flow: the OTP goes to the NEW address (not the current one)
        // so completing it proves ownership of the inbox being switched to. Keycloak isn't
        // touched until ConfirmEmailChangeAsync succeeds - this is what closes the gap that let
        // CandidateProfile.Email and the Keycloak login email drift apart with no verification.
        public async Task<AccountEmailChangeChallengeResponse> RequestEmailChangeAsync(AccountEmailChangeRequest request)
        {
            var user = GetCurrentUser();
            var username = GetUsername(user);
            var userId = await _keycloakClient.GetUserIdByUsernameAsync(username);

            var challengeId = Guid.NewGuid();
            var code = GenerateOtpCode();

            var verification = new EmailChangeVerification
            {
                ChallengeId = challengeId,
                KeycloakUserId = userId,
                NewEmail = request.NewEmail,
                OtpCodeHash = HashOtpCode(code),
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_otpSettings.ExpiryMinutes)
            };

            await _emailChangeVerificationRepository.AddAsync(verification);
            await _unitOfWork.SaveChangesAsync();

            try
            {
                await _notificationDispatchService.DispatchAsync(
                    RecruitmentEventEnum.EmailChangeRequested,
                    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["OtpCode"] = code,
                        ["ExpiryMinutes"] = _otpSettings.ExpiryMinutes.ToString()
                    },
                    new NotificationDispatchTargets(request.NewEmail, null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error dispatching email-change OTP for challenge {ChallengeId}.", challengeId);
            }

            return new AccountEmailChangeChallengeResponse
            {
                ChallengeId = challengeId.ToString(),
                ExpiresAtUtc = verification.ExpiresAtUtc
            };
        }

        public async Task<string> ConfirmEmailChangeAsync(AccountEmailChangeConfirmRequest request)
        {
            if (!Guid.TryParse(request.ChallengeId, out var challengeId))
                throw new OtpVerificationException();

            var verification = await _emailChangeVerificationRepository.GetByChallengeIdAsync(challengeId)
                ?? throw new OtpVerificationException();

            if (verification.Locked || verification.ConsumedAtUtc.HasValue || verification.ExpiresAtUtc < DateTime.UtcNow)
                throw new OtpVerificationException();

            // Only the user who started this challenge may confirm it.
            var user = GetCurrentUser();
            var username = GetUsername(user);
            var userId = await _keycloakClient.GetUserIdByUsernameAsync(username);
            if (userId != verification.KeycloakUserId)
                throw new OtpVerificationException();

            if (!CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(HashOtpCode(request.OtpCode)),
                    Encoding.UTF8.GetBytes(verification.OtpCodeHash)))
            {
                verification.AttemptCount++;
                if (verification.AttemptCount >= _otpSettings.MaxAttempts)
                    verification.Locked = true;

                _emailChangeVerificationRepository.Update(verification);
                await _unitOfWork.SaveChangesAsync();
                throw new OtpVerificationException();
            }

            verification.ConsumedAtUtc = DateTime.UtcNow;
            _emailChangeVerificationRepository.Update(verification);
            await _unitOfWork.SaveChangesAsync();

            // The OTP challenge above already proved ownership of the new address, so mark it
            // verified immediately - no need to also make the user click Keycloak's separate
            // verification-link email.
            await _keycloakClient.UpdateEmailAsync(verification.KeycloakUserId, verification.NewEmail, emailVerified: true);

            if (GetRole(user) == UserRoleEnum.Candidate)
                await _candidateProfileService.SyncVerifiedEmailAsync(verification.KeycloakUserId, verification.NewEmail);

            return verification.NewEmail;
        }

        private static string GenerateOtpCode()
        {
            return RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        }

        private string HashOtpCode(string code)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_otpSettings.Pepper ?? string.Empty));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(code));
            return Convert.ToHexString(hash);
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
            return user.GetHighestRole();
        }
    }
}
